using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{
    public Button ControllerStartButton;
    public Button MouseStartButton;

    void Start()
    {
        ControllerStartButton.onClick.AddListener(OnStartWithController);
        MouseStartButton.onClick.AddListener(OnStartWithMouse);
    }

    private void OnStartWithController()
    {
        SetUseController(true);
        SceneManager.LoadSceneAsync("Game");
    }

    private void OnStartWithMouse()
    {
        SetUseController(false);
        SceneManager.LoadSceneAsync("Game");
    }

    private void SetUseController(bool value)
    {
        FindObjectOfType<GameState>().UseController = value;
    }
}
