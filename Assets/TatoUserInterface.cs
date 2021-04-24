using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TatoUserInterface : MonoBehaviour
{
    private Slider _slider;
    private Worm _worm;

    private void Start()
    {
        _worm = FindObjectOfType<Worm>();
        _slider = FindObjectOfType<Slider>();
    }

    // Update is called once per frame
    private void Update()
    {
        _slider.value = _worm.RemainingLifetimeFraction;
    }
}
