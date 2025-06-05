using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public NivelStats statsUltimoNivel;
    public PartidaStats statsPartida = new PartidaStats();

    public int playerPoints = 100;
    public bool doingSetup;

    //public string nombreJugador;

    //los objetos referentes a la pantalla de información previa a cada partida se crean y se destruyen en cada partida para cambiar los siguientes valores
    private int level = 1;
    public BoardManager boardScript; // generar niveles nuevos con instancias de BoardManager
    public GameObject gameOverPanel;

    private GameObject levelImage;
    private GameObject gameOverPanelInstance;
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
        /*
                textoDerrotaStats = GameObject.Find("OverText")?.GetComponent<Text>();
                botonReintentar = GameObject.Find("ReplayButton")?.GetComponent<Button>();
                botonMenuPrincipal = GameObject.Find("MenuButton")?.GetComponent<Button>();
        */
        if (botonMenuPrincipal != null)
        {
            botonMenuPrincipal.gameObject.SetActive(false);
            Debug.Log("SIIIIIIIIIIIIIIIIIIIIIIII");
        }
        else
        {
            Debug.Log("NOOOOOOOOOOOOOOOOOOOOOOO");
        }
        /*
                if (botonReintentar != null) botonReintentar.gameObject.SetActive(false);
                if (textoDerrotaStats != null) textoDerrotaStats.gameObject.SetActive(false);
        */
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

        // Instanciar el panel desde el prefab
        gameOverPanelInstance = Instantiate(gameOverPanel, Vector3.zero, Quaternion.identity);

        // Asegurar que se añada al Canvas
        gameOverPanelInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);

        // Buscar los botones hijos del panel instanciado
        textoDerrotaStats = gameOverPanelInstance.transform.Find("OverText")?.GetComponent<Text>();
        botonReintentar = gameOverPanelInstance.transform.Find("ReplayButton")?.GetComponent<Button>();
        botonMenuPrincipal = gameOverPanelInstance.transform.Find("MenuButton")?.GetComponent<Button>();

        if (TypingManager.instance != null)
        {
            statsUltimoNivel = TypingManager.instance.ObtenerEstadisticasActuales();
            float duracion = TypingManager.instance.ObtenerDuracion();

            statsPartida.Agregar(statsUltimoNivel, duracion);
        }


        if (statsPartida != null && textoDerrotaStats != null)
        {
            float precision = statsPartida.CalcularPrecision();
            float velocidad = statsPartida.CalcularVelocidad();

            string resumen = $"<b>⚰️ Derrota tras el nivel {level}</b>\n\n" +
                            $"<b>Palabras totales:</b> {statsPartida.totalPalabras}\n" +
                            $"<b>Letras correctas:</b> {statsPartida.totalLetrasCorrectas}\n" +
                            $"<b>Letras incorrectas:</b> {statsPartida.totalLetrasIncorrectas}\n" +
                            $"<b>Precisión:</b> {precision:F1}%\n" +
                            $"<b>Velocidad media:</b> {velocidad:F1} ppm";

            textoDerrotaStats.text = resumen;


            if (botonReintentar != null)
                botonReintentar.onClick.AddListener(Reintentar);

            if (botonMenuPrincipal != null)
                botonMenuPrincipal.onClick.AddListener(IrAlMenu);
        }
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

    public void Reintentar()
    {
        Debug.Log("🔄 Reintentando el nivel...");
        Time.timeScale = 1f;

        // Reiniciar valores de juego
        playerPoints = 100;
        level = 1;
        statsUltimoNivel = null;
        statsPartida = new PartidaStats(); // Reiniciar estadísticas acumuladas


        SceneManager.LoadScene("Juego");
    }

    public void IrAlMenu()
    {
        Debug.Log("🏠 Volviendo al menú principal...");
        Time.timeScale = 1f;

        // Reiniciar progreso
        level = 1;
        playerPoints = 100;
        statsUltimoNivel = null;
        statsPartida = new PartidaStats(); // Reiniciar estadísticas acumulada

        SceneManager.LoadScene("MenuPrincipal");
    }
    public string GetDificultadSeleccionada()
    {
        return PlayerPrefs.GetString("dificultad_seleccionada", "fácil"); // por defecto "facil"
    }

    public string GetTipoPorNivel()
    {
        int nivel = GetLevel();
        if (nivel <= 5) return "letra";
        else if (nivel <= 10) return "palabra";
        else
        {
            int r = UnityEngine.Random.Range(0, 3);
            return r == 0 ? "letra" : r == 1 ? "palabra" : "frase";
        }
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

//clase auxiliar para acumular los datos
public class PartidaStats
{
    public int totalPalabras = 0;
    public int totalLetrasCorrectas = 0;
    public int totalLetrasIncorrectas = 0;
    public float tiempoTotal = 0f;

    public void Agregar(NivelStats nivel, float duracionNivel)
    {
        totalPalabras += nivel.palabras;
        totalLetrasCorrectas += nivel.letrasCorrectas;
        totalLetrasIncorrectas += nivel.letrasIncorrectas;
        tiempoTotal += duracionNivel;
    }

    public float CalcularPrecision()
    {
        int total = totalLetrasCorrectas + totalLetrasIncorrectas;
        return total > 0 ? (totalLetrasCorrectas / (float)total) * 100f : 100f;
    }

    public float CalcularVelocidad()
    {
        return tiempoTotal > 0f ? totalPalabras / (tiempoTotal / 60f) : 0f;
    }
}
