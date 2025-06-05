using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipalManager : MonoBehaviour
{
    [Header("Botones del menú")]
    public Button jugarButton;
    public Button tutorialButton;
    public Button estadisticasButton;
    public Button configuracionButton;
    public Button cerrarSesionButton;
    public Button salirButton;

    [Header("Panel de Tutorial")]
    public TutorialManager tutorialManager;

    [Header("Panel de Estadísticas")]
    public FullStatsScreenManager estadisticasManager;

    [Header("Panel de Configuración")]
    public ConfigurationManager configuraManager;

    void Start()
    {
        if (jugarButton != null)
            jugarButton.onClick.AddListener(CargarJuego);

        if (tutorialButton != null)
            tutorialButton.onClick.AddListener(AbrirTutorial);

        if (estadisticasButton != null)
            estadisticasButton.onClick.AddListener(MostrarEstadisticas);

        if (configuracionButton != null)
            configuracionButton.onClick.AddListener(AbrirConfiguracion);

        if (cerrarSesionButton != null)
            cerrarSesionButton.onClick.AddListener(CerrarSesion);

        if (salirButton != null)
            salirButton.onClick.AddListener(SalirDelJuego);
    }

    void CargarJuego()
    {
        SceneManager.LoadScene("Juego"); // Asegúrate que la escena está en el Build Settings
    }

    void AbrirTutorial()
    {
        if (tutorialManager != null)
        {
            tutorialManager.MostrarTutorial();
        }
        else
        {
            Debug.LogWarning("⚠️ No se asignó TutorialManager en el inspector.");
        }
    }

    void MostrarEstadisticas()
    {
        Debug.Log("📊 Botón de estadísticas presionado");

        if (estadisticasManager != null)
        {
            estadisticasManager.MostrarEstadisticas();
        }
        else
        {
            Debug.LogWarning("⚠️ No se asignó TutorialManager en el inspector.");
        }
    }

    void AbrirConfiguracion()
    {
        if (tutorialManager != null)
        {
            configuraManager.MostrarConfiguracion();
        }
        else
        {
            Debug.LogWarning("⚠️ No se asignó ConfigurationManager en el inspector.");
        }
    }

    void CerrarSesion()
    {
        Debug.Log("🔒 Cierre de sesión");
        PlayerPrefs.DeleteKey("usuario_seleccionado"); // Opcional: limpiar usuario guardado
        SceneManager.LoadScene("Inicio");
    }

    void SalirDelJuego()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

