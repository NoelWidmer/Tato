using UnityEngine;

public class GameState : MonoBehaviour
{
    public bool UseController;
    public int PersonalBest;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
