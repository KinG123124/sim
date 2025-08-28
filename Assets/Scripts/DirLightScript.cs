using UnityEngine;

public class DirLightScript : MonoBehaviour
{
    [SerializeField] private GameObject _sun;
    [SerializeField] private GameObject _moon;
    [SerializeField] private float rotationSpeed = 1f;

    private float _currentAngle = 0f;

    void Update()
    {
        // Вращаем вручную, чтобы точно контролировать угол
        _currentAngle += rotationSpeed * Time.deltaTime;
        _currentAngle %= 360; // Удерживаем в диапазоне 0–360

        // Применяем вращение к объекту
        transform.rotation = Quaternion.Euler(_currentAngle, 0, 0);

        // Переключение между солнцем и луной
        if (_currentAngle >= 100f && _currentAngle < 260f)
        {
            EnableMoon(); // Ночь (100°–260°)
        }
        else
        {
            EnableSun(); // День (260°–100°)
        }
    }

    void EnableMoon()
    {
        if (!_moon.activeSelf) _moon.SetActive(true);
        if (_sun.activeSelf) _sun.SetActive(false);
    }

    void EnableSun()
    {
        if (!_sun.activeSelf) _sun.SetActive(true);
        if (_moon.activeSelf) _moon.SetActive(false);
    }
}