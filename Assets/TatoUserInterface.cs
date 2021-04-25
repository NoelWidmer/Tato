using UnityEngine;
using UnityEngine.UI;

public class TatoUserInterface : MonoBehaviour
{
    public Text DepthValue;
    public Text StarValue;

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
        DepthValue.text = _worm.Depth.ToString();
        StarValue.text = _worm.Stars.ToString();
    }
}
