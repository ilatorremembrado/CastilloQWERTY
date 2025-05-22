using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public BoardManager boardScript; // generar niveles nuevos con instancias de BoardManager
    public int playerPoints = 100;
	//public float turnDelay = 0.1f;//tiempo a esperar entre movimientos (duración de un turno)
	public bool doingSetup;

//los objetos referentes a la pantalla de información previa a cada partida se crean y se destruyen en cada partida para cambiar los siguientes valores
    private int level = 0;
    private GameObject levelImage;
    private Text levelText;

    void Awake()
    {
	//singleton: permite acceder al gamemanager desde cualquier script usando la variable instance
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

	//para que el gamemanager no se destruya al generar otra escena o recargar la misma
        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManager>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        level++;
        InitGame();
    }

    void InitGame()
    {
	// prepara la pantalla prejuego
	doingSetup = true; // para que el jugador no se pueda mover mientras se prepara la escena
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        levelText.text = "Day " + level;
        levelImage.SetActive(true);

	// prepara escena juego
        boardScript.SetupScene(level);

	//en (levelStartDelay) segundos se genera el método (HideLevelImage)
        Invoke("HideLevelImage", 2f);
    }

    void HideLevelImage()
    {
        levelImage.SetActive(false);
	doingSetup = false;
    }

    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        Time.timeScale = 0f;
    }
}