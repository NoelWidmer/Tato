using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{
    public Button StartButton;

    void Start()
    {
        StartButton.onClick.AddListener(OnStart);
    }

    private void OnStart()
    {
        SceneManager.LoadSceneAsync("Game");
    }
}
