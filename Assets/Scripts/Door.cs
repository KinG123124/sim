using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;     // Скорость вращения (°/сек)
    [SerializeField] private float openAngle = 150f;       // Угол открытия
    [SerializeField] private AudioClip openSound;          // Звук открытия
    [SerializeField] private AudioClip closeSound;         // Звук закрытия

    private bool isOpen = false;
    private bool isRotating = false;
    private Quaternion targetRotation;
    private AudioSource audioSource;                       // Компонент AudioSource

    void Start()
    {
        // Получаем компонент AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource не найден на объекте двери!");
        }
    }

    public void ToggleDoor()
    {
        if (isRotating) return;

        isRotating = true;
        isOpen = !isOpen;

        // Выбираем звук в зависимости от состояния
        AudioClip clip = isOpen ? openSound : closeSound;
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip); // Проигрываем звук
        }

        // Вычисляем целевой поворот
        float angle = isOpen ? openAngle : -openAngle;
        targetRotation = transform.rotation * Quaternion.Euler(0, angle, 0);
    }

    void Update()
    {
        if (isRotating)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                isRotating = false;
            }
        }
    }
}