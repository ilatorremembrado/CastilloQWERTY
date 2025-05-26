using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Text levelText;
    public float delayBeforeStart = 2f;

    void Start()
    {
        // Asume que GameManager ya tiene el nivel
        int nivel = GameManager.instance != null ? GameManager.instance.GetLevel() : 1;
        levelText.text = "Nivel " + nivel;

        Invoke("CargarJuego", delayBeforeStart);
    }

    void CargarJuego()
    {
        SceneManager.LoadScene("Juego");
    }
}
