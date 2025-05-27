using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int playerPoints = 100;
    public bool doingSetup;

    //public string nombreJugador;

    //los objetos referentes a la pantalla de información previa a cada partida se crean y se destruyen en cada partida para cambiar los siguientes valores
    private int level = 0;
    public BoardManager boardScript; // generar niveles nuevos con instancias de BoardManager
    private GameObject levelImage;
    private Text levelText;

    void Awake()
    {   
        //singleton: permite acceder al gamemanager desde cualquier script usando la variable instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Juego")
        {
            InitGame();
        }
    }

    public void InitGame()
    {
        // prepara la pantalla prejuego
        doingSetup = true; // para que el jugador no se pueda mover mientras se prepara la escena
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText")?.GetComponent<Text>();

        if (levelText != null) levelText.text = "Nivel: " + level;
        if (levelImage != null) levelImage.SetActive(true);

        // prepara escena juego
        boardScript = GetComponent<BoardManager>();
        if (boardScript != null) boardScript.SetupScene(level);

        //en (levelStartDelay) segundos se genera el método (HideLevelImage)
        Invoke("HideLevelImage", 2f);
    }

    void HideLevelImage()
    {
        if (levelImage != null) levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        doingSetup = true;

        if (levelText != null) levelText.text = "Despues de " + level + " salas, te derrotaron.";
        if (levelImage != null) levelImage.SetActive(true);

        Time.timeScale = 0f;
    }
    
    public int GetLevel() => level;
    public string GetPlayerName() => PlayerPrefs.GetString("usuario_seleccionado");

}