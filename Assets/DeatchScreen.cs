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

        Score.text = StarsCollectedText.text = starCount.ToString();
        PotatoText.text = depth.ToString();
        LengthText.text = length.ToString();
        TimeText.text = time.ToString();
    }

    private void OnRestart()
    {
        Debug.Log("restart");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
}
