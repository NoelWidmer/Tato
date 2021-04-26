using UnityEngine;
using UnityEngine.UI;

public class TatoUserInterface : MonoBehaviour
{
    public Image ColoredBar;
    public Color Green;
    public Color Orange;
    public Color Red;

    public Text ScoreLabel;
    public Text ScoreValue;
    public Color HighScoreColor;

    private Slider _slider;
    private Worm _worm;
    private GameState _gameState;

    private void Start()
    {
        _worm = FindObjectOfType<Worm>();
        _slider = FindObjectOfType<Slider>();
        _gameState = FindObjectOfType<GameState>();
    }

    private void LateUpdate()
    {
        ColoredBar.color = _worm.RemainingLifetimeFraction < .3f ? Red : _worm.RemainingLifetimeFraction < .7f ? Orange : Green;
        _slider.value = _worm.RemainingLifetimeFraction;

        ScoreValue.text = _worm.Stars.ToString();

        if (_worm.Stars > _gameState.HighScore)
        {
            ScoreLabel.text = "New High Score!";
            ScoreValue.color = ScoreLabel.color = HighScoreColor;
        }
    }
}
