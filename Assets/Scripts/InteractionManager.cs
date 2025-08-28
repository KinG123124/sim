using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite noInteractive;
    [SerializeField] private Sprite Interactive;

    [System.Serializable]
    public class InteractionEvent : UnityEvent { }

    [Header("��������� ��������������")]
    public LayerMask interactableLayer; // ���� ��� ������������� ��������
    public float maxInteractionDistance = 5f; // ������������ ���������� ��������������
    public KeyCode interactionKey = KeyCode.E; // ������� ��� ��������������

    [Header("�������")]
    public InteractionEvent onInteract; // ������� ��� ��������������

    private GameObject currentInteractable; // ������� ������������� ������
    private Camera mainCamera; // �������� ������
    public CarController _controller;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    void Update()
    {
        CheckInteraction();
        HandleInput();
    }

    private void CheckInteraction()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // ���������, ����� �� ��� �� ������������� ������
        if (Physics.Raycast(ray, out hit, maxInteractionDistance, interactableLayer))
        {
            currentInteractable = hit.collider.gameObject;
            Debug.Log("Hovered over: " + currentInteractable.name);
            image.sprite = Interactive;
        }
        else
        {
            image.sprite = noInteractive;
            currentInteractable = null;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(interactionKey) && currentInteractable != null)
        {
            Door door = currentInteractable.GetComponent<Door>();
            if (door != null)
                door.ToggleDoor();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_controller != null)
            {
                _controller.ExitCar();
                _controller = null;
            }
            else if (currentInteractable != null)
            {
                Debug.Log("Interacting with: " + currentInteractable.name);
                onInteract?.Invoke();

                CarController carController = currentInteractable.GetComponent<CarController>();
                if (carController != null)
                {

                    float distance = Vector3.Distance(carController.transform.position, carController.player.transform.position);
                    Debug.Log("Player distance: " + distance); // ������� ����������
                    if (distance < 5f) // ��������� �� 5 ������
                    {
                        if (!carController.isPlayerInCar)
                        {
                            carController.EnterCar();
                            _controller = carController;
                            Debug.Log("Entered car");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Player too far from car! (Distance: " + distance + ")");
                    }
                }
                else
                {
                    Debug.LogWarning(currentInteractable.name + " has no CarController component!");
                }
            }
        }
    }
}