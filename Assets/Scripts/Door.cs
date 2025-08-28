using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;     // �������� �������� (�/���)
    [SerializeField] private float openAngle = 150f;       // ���� ��������
    [SerializeField] private AudioClip openSound;          // ���� ��������
    [SerializeField] private AudioClip closeSound;         // ���� ��������

    private bool isOpen = false;
    private bool isRotating = false;
    private Quaternion targetRotation;
    private AudioSource audioSource;                       // ��������� AudioSource

    void Start()
    {
        // �������� ��������� AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource �� ������ �� ������� �����!");
        }
    }

    public void ToggleDoor()
    {
        if (isRotating) return;

        isRotating = true;
        isOpen = !isOpen;

        // �������� ���� � ����������� �� ���������
        AudioClip clip = isOpen ? openSound : closeSound;
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip); // ����������� ����
        }

        // ��������� ������� �������
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