using Cinemachine;
using StarterAssets;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Колёса")]
    public Wheel[] wheels;
    public GameObject centerOfMassObject;

    [Header("Ссылки на игрока")]
    public FirstPersonController firstPersonController;
    public StarterAssetsInputs starterInputs;
    public GameObject player;
    public Transform exitPoint;
    public CinemachineVirtualCamera playerCam;
    public CinemachineVirtualCamera carCam;

    [Header("Настройки машины")]
    public float maxMotorTorque = 1000f;
    public float maxSteerAngle = 40f;
    public float brakeTorque = 2000f;

    [Header("Фары")]
    private int _LightMode = 0; // 0-выкл 1-ближний 2-дальний
    [SerializeField] Light[] _Lights;

    public bool isPlayerInCar = false;

    private float vInput;
    private float hInput;
    public Rigidbody rb;

    [Header("Бинды")]
    public KeyCode HeadlightsKey = KeyCode.H;
    public KeyCode EngineKey = KeyCode.B; 


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (centerOfMassObject != null)
        {
            rb.centerOfMass = transform.InverseTransformPoint(centerOfMassObject.transform.position);
        }

        // при старте машина выключена
        isPlayerInCar = false;
    }

    void FixedUpdate()
    {
        if (isPlayerInCar)
        {
            vInput = Input.GetAxis("Vertical");
            hInput = Input.GetAxis("Horizontal");

            Drive();
            Steer();
            UpdateWheelPoses();
        }
    }

    private void Update()
    {
        if (isPlayerInCar && Input.GetKeyDown(HeadlightsKey))
            changeLightMode();
    }

    // --- ВХОД В МАШИНУ ---
    public void EnterCar()
    {
        firstPersonController.enabled = false;
        starterInputs.enabled = false;
        player.SetActive(false);

        isPlayerInCar = true;

        playerCam.Priority = 0;
        carCam.Priority = 10; // выше → активна
    }

    public void ExitCar()
    {
        firstPersonController.enabled = true;
        starterInputs.enabled = true;
        player.SetActive(true);

        player.transform.position = exitPoint.position;

        isPlayerInCar = false;

        playerCam.Priority = 10;
        carCam.Priority = 0;
    }

    // --- ДВИЖЕНИЕ ---
    private void Drive()
    {
        float motorTorque = vInput * maxMotorTorque; // теперь W = вперёд, S = назад
        float brake = Input.GetKey(KeyCode.Space) ? brakeTorque : 0f; // тормоз только на пробел

        foreach (var wheel in wheels)
        {
            if (wheel.DriveWheel)
            {
                wheel._WheelCollider.motorTorque = -motorTorque;
                wheel._WheelCollider.brakeTorque = brake;
            }
        }
    }

    private void Steer()
    {
        float steerAngle = hInput * maxSteerAngle;
        foreach (var wheel in wheels)
        {
            if (wheel.SteerWheel)
            {
                wheel._WheelCollider.steerAngle = steerAngle;
            }
        }
    }

    private void UpdateWheelPoses()
    {
        foreach (var wheel in wheels)
        {
            UpdateWheelPose(wheel._WheelCollider, wheel._WheelMesh);
        }
    }

    private void UpdateWheelPose(WheelCollider collider, GameObject wheelMesh)
    {
        if (wheelMesh == null || collider == null) return;

        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);

        wheelMesh.transform.position = pos;
        wheelMesh.transform.rotation = rot;
    }

    private void changeLightMode()
    {
        switch (_LightMode)
        {
            case 0: // ближний
                foreach (var Light in _Lights)
                {
                    Light.intensity = 5 * 10000;
                    Light.range = 40;
                }
                _LightMode++;
                break;

            case 1: // дальний
                foreach (var Light in _Lights)
                {
                    Light.intensity = 8 * 10000;
                    Light.range = 80;
                }
                _LightMode++;
                break;

            case 2: // выкл
                foreach (var Light in _Lights)
                {
                    Light.intensity = 0;
                }
                _LightMode = 0;
                break;
        }
    }
}

[System.Serializable]
public struct Wheel
{
    public WheelCollider _WheelCollider;
    public GameObject _WheelMesh;
    public bool DriveWheel;
    public bool SteerWheel;
}
