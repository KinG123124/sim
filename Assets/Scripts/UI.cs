using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private InteractionManager IM;
    [SerializeField] private TextMeshProUGUI speedometer;
    [SerializeField] private AnimationCurve Takhometer;
    [SerializeField] private Image strelkaTakhometer;
    [SerializeField] private GameObject CarGUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //float speed = controller.velocity.magnitude;
        //if (speed == 0)
        //    try
        //    {
        //        speed = IM._controller.rb.linearVelocity.magnitude;
        //    }
        //    catch { }
        float speed = 0;

        if (IM._controller != null)
        {
            speed = IM._controller.rb.linearVelocity.magnitude;
            CarGUI.SetActive(true);
        }
        else CarGUI.SetActive(false);

        speed = Mathf.Round(speed * 2) ;
        speedometer.text = speed + "";
        var rot = strelkaTakhometer.rectTransform.localRotation.eulerAngles;
        rot.z = Takhometer.Evaluate(speed);
        strelkaTakhometer.rectTransform.localRotation = Quaternion.Euler(rot);
    }
}
