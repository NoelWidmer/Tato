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

    private void LateUpdate()
    {
        _slider.value = _worm.RemainingLifetimeFraction;
    }
}
