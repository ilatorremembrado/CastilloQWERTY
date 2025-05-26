using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.InitGame();
        }
    }
}
