using UnityEngine;

public class LightSwitcher : MonoBehaviour
{
    [SerializeField] Light[] _Lights;
    [SerializeField] Material mat;
    private bool toggle;

    void Start()
    {
        for (int i = 0; i < _Lights.Length; i++)
        {
            _Lights[i].intensity = 0;
        }
    }

    void Update()
    {

    }
    public void SwitchMode()
    {
        toggle = !toggle;
        if (toggle)
        {
            for (int i = 0; i < _Lights.Length; i++)
            {
                _Lights[i].intensity = 3000;

            }
        }
        else
        {
            for (int i = 0; i < _Lights.Length; i++)
            {
                _Lights[i].intensity = 0;
            }
        }
        var rot = transform.rotation.eulerAngles;
        rot.z += 180;
        transform.rotation = Quaternion.Euler(rot);
    }
}
