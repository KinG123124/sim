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

    [Header("Настройки взаимодействия")]
    public LayerMask interactableLayer; // Слой для интерактивных объектов
    public float maxInteractionDistance = 5f; // Максимальное расстояние взаимодействия
    public KeyCode interactionKey = KeyCode.E; // Клавиша для взаимодействия

    [Header("События")]
    public InteractionEvent onInteract; // Событие при взаимодействии

    private GameObject currentInteractable; // Текущий интерактивный объект
    private Camera mainCamera; // Основная камера
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

        // Проверяем, попал ли луч на интерактивный объект
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
                    UpdateInteractText("Сесть [F]");
                    return;
                }
            }
            Door door = currentInteractable.GetComponent<Door>();
            if (door != null)
            {
                UpdateInteractText("Дверь [E]");
                return;
            }
            LightSwitcher switcher = currentInteractable.GetComponent<LightSwitcher>();
            if (switcher != null)
            {
                UpdateInteractText("Свет [E]");
                return;
            }
        }
        else // Луч не попал на интерактивный объект
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
                    Debug.Log("Player distance: " + distance); // Отладка расстояния
                    if (distance < 5f) // Увеличено до 5 единиц
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