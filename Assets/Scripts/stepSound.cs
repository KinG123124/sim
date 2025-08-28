using UnityEngine;
using System.Collections;

public class StepSound : MonoBehaviour
{
    [Header("Sound Settings")]
    public AudioClip stepSoundClip;
    public float minStepInterval = 0.4f;  // Увеличенный интервал (в 2 раза)
    public float maxStepInterval = 1.2f;  // Увеличенный интервал (в 2 раза)
    public float minSpeed = 0.1f;
    public float maxSpeed = 5f;
    [Range(0f, 1f)] public float volume = 0.5f;

    [Header("Fade Settings")]
    public float fadeOutTime = 0.2f;

    [Header("Advanced")]
    public float minPitch = 0.8f;       // Минимальная высота тона (при медленной ходьбе)
    public float maxPitch = 1.2f;       // Максимальная высота тона (при беге)
    public float pitchVariation = 0.1f;  // Случайное отклонение тона
    public bool checkGrounded = true;

    private AudioSource audioSource;
    private CharacterController characterController;
    private float stepTimer;
    private bool isFadingOut;

    void Start()
    {
        if (stepSoundClip == null)
            Debug.LogError("AudioClip для шага не назначен!", this);

        characterController = GetComponent<CharacterController>();
        if (characterController == null)
            Debug.LogError("CharacterController не найден!", this);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = stepSoundClip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
    }

    void Update()
    {
        bool isGrounded = !checkGrounded || characterController.isGrounded;
        Vector3 velocity = characterController.velocity;
        velocity.y = 0f;
        float currentSpeed = velocity.magnitude;

        if (currentSpeed > minSpeed && isGrounded)
        {
            if (isFadingOut)
            {
                StopAllCoroutines();
                audioSource.volume = volume;
                isFadingOut = false;
            }

            // Динамический расчет интервала и pitch
            float speedRatio = Mathf.Clamp01((currentSpeed - minSpeed) / (maxSpeed - minSpeed));
            float currentStepInterval = Mathf.Lerp(maxStepInterval, minStepInterval, speedRatio);
            float currentPitch = Mathf.Lerp(minPitch, maxPitch, speedRatio) + Random.Range(-pitchVariation, pitchVariation);

            audioSource.pitch = currentPitch; // Применяем pitch

            stepTimer += Time.deltaTime;
            if (stepTimer >= currentStepInterval)
            {
                stepTimer = 0f;
                PlayStepSound();
            }
        }
        else
        {
            stepTimer = 0f;
            if (audioSource.isPlaying && !isFadingOut)
                StartCoroutine(FadeOutSound());
        }
    }

    void PlayStepSound()
    {
        audioSource.Stop();
        audioSource.Play();
    }

    IEnumerator FadeOutSound()
    {
        isFadingOut = true;
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeOutTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = volume;
        isFadingOut = false;
    }
}