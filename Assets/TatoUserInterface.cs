using UnityEngine;
using UnityEngine.UI;

public class TatoUserInterface : MonoBehaviour
{
    public Image ColoredBar;
    public Color Green;
    public Color Orange;
    public Color Red;

    private Slider _slider;
    private Worm _worm;

    private void Start()
    {
        _worm = FindObjectOfType<Worm>();
        _slider = FindObjectOfType<Slider>();
    }

    private void LateUpdate()
    {
        ColoredBar.color = _worm.RemainingLifetimeFraction < .3f ? Red : _worm.RemainingLifetimeFraction < .7f ? Orange : Green;
        _slider.value = _worm.RemainingLifetimeFraction;
    }
}
