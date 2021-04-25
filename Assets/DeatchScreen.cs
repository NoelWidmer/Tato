using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeatchScreen : MonoBehaviour
{
    public Text HeaderText;
    public Text HintText;

    public Text Score;

    public Text StarsCollectedText;
    public Text PotatoText;
    public Text LengthText;
    public Text TimeText;

    public Button RestartButton;

    private void Start()
    {
        RestartButton.onClick.AddListener(OnRestart);
    }

    public void SetValues(string headerText, string hinttext, int starCount, int depth, int length, float time)
    {
        HeaderText.text = headerText;
        HintText.text = hinttext;

        var timeSpan = TimeSpan.FromMilliseconds(Mathf.FloorToInt(time * 1000));

        Score.text = StarsCollectedText.text = starCount.ToString();
        PotatoText.text = depth.ToString();
        LengthText.text = length.ToString();
        TimeText.text = $"{timeSpan.Minutes}m {timeSpan.Seconds}s {timeSpan.Milliseconds}ms";
    }

    private void OnRestart()
    {
        Debug.Log("restart");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
}
