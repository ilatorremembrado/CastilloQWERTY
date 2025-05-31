using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public NivelStats statsUltimoNivel;
    public int playerPoints = 100;
    public bool doingSetup;

    //public string nombreJugador;

    //los objetos referentes a la pantalla de información previa a cada partida se crean y se destruyen en cada partida para cambiar los siguientes valores
    private int level = 1;
    public BoardManager boardScript; // generar niveles nuevos con instancias de BoardManager
    private GameObject levelImage;
    private Text levelText;
    private Text textoDerrotaStats;
    private Button botonReintentar;
    private Button botonMenuPrincipal;


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

        textoDerrotaStats = GameObject.Find("OverText")?.GetComponent<Text>();
        botonReintentar = GameObject.Find("ReintentarButton")?.GetComponent<Button>();
        botonMenuPrincipal = GameObject.Find("MenuButton")?.GetComponent<Button>();

        if (botonMenuPrincipal != null)
        {
            botonMenuPrincipal.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("no se encuentran los botones");
        }

        if (botonReintentar != null) botonReintentar.gameObject.SetActive(false);
        if (textoDerrotaStats != null) textoDerrotaStats.gameObject.SetActive(false);

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

        GuardarPartida();


        if (levelText != null) levelText.text = "";
        
        if (botonMenuPrincipal != null) botonMenuPrincipal.gameObject.SetActive(true);
        if (botonReintentar != null) botonReintentar.gameObject.SetActive(true);
        if (textoDerrotaStats != null) textoDerrotaStats.gameObject.SetActive(true);


        if (statsUltimoNivel != null && textoDerrotaStats != null)
        {
            string resumen = $"Después de {level} niveles, te derrotaron\n" +
                            $"<b>Palabras:</b> {statsUltimoNivel.palabras}\n" +
                            $"<b>Correctas:</b> {statsUltimoNivel.letrasCorrectas}\n" +
                            $"<b>Errores:</b> {statsUltimoNivel.letrasIncorrectas}\n" +
                            $"<b>Precisión:</b> {statsUltimoNivel.precision:F1}%\n" +
                            $"<b>Velocidad:</b> {statsUltimoNivel.velocidad:F1} ppm";

            textoDerrotaStats.text = resumen;


            if (botonReintentar != null)
                botonReintentar.onClick.AddListener(ReintentarNivel);

            if (botonMenuPrincipal != null)
                botonMenuPrincipal.onClick.AddListener(IrAlMenu);
        }
         if (levelImage != null) levelImage.SetActive(true);
        Time.timeScale = 0f;
    }

    void GuardarPartida()
    {
        string nombre = GetPlayerName();

        float velocidad = TypingManager.instance != null ? TypingManager.instance.CalcularVelocidad() : 0f;
        float precision = TypingManager.instance != null ? TypingManager.instance.CalcularPrecision() : 100f;
        int puntuacion = playerPoints;

        var datos = new PartidaAPI.PartidaRequest
        {
            nombre = nombre,
            nivel = level,
            puntuacion = puntuacion,
            velocidad = velocidad,
            precision = precision
        };

        // Llamada a la API
        MonoBehaviour mono = Object.FindFirstObjectByType<Player>();
        mono.StartCoroutine(PartidaAPI.EnviarPartida(datos, (ok, msg) =>
        {
            Debug.Log(ok ? "✅ Partida registrada" : "❌ Error registrando partida: " + msg);
        }));
    }

    public int GetLevel() => level;
    public string GetPlayerName() => PlayerPrefs.GetString("usuario_seleccionado");

    public IEnumerator IrAlSiguienteNivel()
    {
        doingSetup = true;

        yield return new WaitForSeconds(2f);

        level++; // Incrementa el número de nivel
        Time.timeScale = 1f;

        SceneManager.LoadScene("Juego");
    }

    public void ReintentarNivel()
    {
        Debug.Log("🔄 Reintentando el nivel...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Juego");
    }

    public void IrAlMenu()
    {
        Debug.Log("🏠 Volviendo al menú principal...");
        Time.timeScale = 1f;
        level = 1; // O reiniciar progreso si lo deseas
        SceneManager.LoadScene("MenuPrincipal");
    }


}

//clase auxiliar
public class NivelStats
{
    public int palabras;
    public int letrasCorrectas;
    public int letrasIncorrectas;
    public float velocidad;
    public float precision;
}
