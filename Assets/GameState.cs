using UnityEngine;

public class GameState : MonoBehaviour
{
    public bool UseController;
    public int HighScore;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
