using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeatchScreen : MonoBehaviour
{
    public Text HeaderText;
    public Text HintText;

    public Text Score;
    public Text HighScore;

    public Text RainText;
    public Text PotatoText;
    public Text LengthText;
    public Text TimeText;

    public Button RestartButton;

    private void Start()
    {
        RestartButton.onClick.AddListener(OnRestart);
    }

    public void SetValues(string title, string subtitle, int score, int layer, int length, float playtime)
    {
        var gameState = FindObjectOfType<GameState>();

        HeaderText.text = title;
        HintText.text = subtitle;

        var timeSpan = TimeSpan.FromMilliseconds(Mathf.FloorToInt(playtime * 1000));

        Score.text = score.ToString();

        if (score > gameState.HighScore)
        {
            gameState.HighScore = score;
            HighScore.text = "New High Score!";
        }
        else
        {
            HighScore.text = $"High Score: {gameState.HighScore}";
        }

        RainText.text = (layer - 1).ToString();
        PotatoText.text = layer.ToString();
        LengthText.text = length.ToString();
        TimeText.text = $"{timeSpan.Minutes}m {timeSpan.Seconds}s {timeSpan.Milliseconds}ms";
    }

    private void OnRestart()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
}
