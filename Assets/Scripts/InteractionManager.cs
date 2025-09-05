using System;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite noInteractive;
    [SerializeField] private Sprite Interactive;
    [SerializeField] private TextMeshProUGUI InteractText;
    [SerializeField] private GameObject playerObj;

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

    public NavMeshAgent NPC;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

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
        if (Input.GetKeyDown(KeyCode.Z)) GetNpc();
    }

    private void GetNpc()
    {
        NPC.SetDestination(playerObj.transform.position);
    }

    public void UpdateInteractText(string newText = "")
    {
        if (newText != "")
        {
            InteractText.text = newText;
            InteractText.gameObject.SetActive(true);
        }
        else InteractText.gameObject.SetActive(false);
    }
    private void CheckInteraction()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // ���������, ����� �� ��� �� ������������� ������
        if (Physics.Raycast(ray, out hit, maxInteractionDistance))
        {
            if (!hit.collider.gameObject.CompareTag("Interactable")) return;

            currentInteractable = hit.collider.gameObject;
            Debug.Log("Hovered over: " + currentInteractable.name);
            image.sprite = Interactive;
            CarController carController = currentInteractable.GetComponent<CarController>();
            if (carController != null)
            {
                if (_controller == null)
                {
                    UpdateInteractText("����� [F]");
                    return;
                }
            }
            Door door = currentInteractable.GetComponent<Door>();
            if (door != null)
            {
                UpdateInteractText("����� [E]");
                return;
            }
            LightSwitcher switcher = currentInteractable.GetComponent<LightSwitcher>();
            if (switcher != null)
            {
                UpdateInteractText("���� [E]");
                return;
            }
        }
        else // ��� �� ����� �� ������������� ������
        {
            image.sprite = noInteractive;
            currentInteractable = null;
            UpdateInteractText();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(interactionKey) && currentInteractable != null)
        {
            Door door = currentInteractable.GetComponent<Door>();
            if (door != null)
            {
                door.ToggleDoor();
                return;
            }
            LightSwitcher switcher = currentInteractable.GetComponent<LightSwitcher>();
            if (switcher != null)
            {
                switcher.SwitchMode();
            }
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