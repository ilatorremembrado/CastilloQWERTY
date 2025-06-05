using UnityEngine;
using UnityEngine.UI;

public class ConfigurationManager : MonoBehaviour
{
    [Header("Panel y componentes")]
    public GameObject configurationPanel;
    public Button buttonCerrar;
    public Button buttonFacil;
    public Button buttonMedio;
    public Button buttonDificil;
    public Button buttonAceptar;

    private string dificultadSeleccionada;
    void Start()
    {
        // Asegurar que está inactivo al inicio
        configurationPanel.SetActive(false);

        // Asignar eventos a botones
        buttonCerrar.onClick.AddListener(Cerrar);
        buttonFacil.onClick.AddListener(() =>
        {
            dificultadSeleccionada = "fácil";
        });
        buttonMedio.onClick.AddListener(() =>
        {
            dificultadSeleccionada = "media";
        });
        buttonDificil.onClick.AddListener(() =>
        {
            dificultadSeleccionada = "difícil";
        });
        buttonAceptar.onClick.AddListener(() =>
        {
            OnElegirDificultad();
            Cerrar();
        });
        
    }

    public void MostrarConfiguracion()
    {
        configurationPanel.SetActive(true);
    }

    void OnElegirDificultad()
    {
        PlayerPrefs.SetString("dificultad_seleccionada", dificultadSeleccionada);
        PlayerPrefs.Save();
        Debug.Log("🎯 Dificultad guardada: " + dificultadSeleccionada);
    }

    void Cerrar()
    {
        configurationPanel.SetActive(false);
    }

}
