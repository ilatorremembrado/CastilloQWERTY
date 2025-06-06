using System.Collections.Generic;
using UnityEngine;

public class LogrosManager : MonoBehaviour
{
    public static LogrosManager instance;

    private List<Logro> logros = new List<Logro>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DefinirLogros();
    }

    void DefinirLogros()
    {
        logros.Add(new Logro(
            "primer_partida",
            "Juega tu primera partida",
            () => GameManager.instance.statsPartida.totalPalabras > 0
        ));

        logros.Add(new Logro(
            "cien_palabras",
            "Escribe 100 palabras en total",
            () => GameManager.instance.statsPartida.totalPalabras >= 100
        ));

        logros.Add(new Logro(
            "preciso_90",
            "Termina con precisión mayor al 90%",
            () => GameManager.instance.statsPartida.CalcularPrecision() > 90f
        ));
    }

    public void RevisarLogros()
    {
        foreach (var logro in logros)
        {
            if (!logro.desbloqueado && logro.condicion())
            {
                logro.desbloqueado = true;
                GuardarLogro(logro.descripcion);
                MostrarLogroUI(logro.descripcion);
                Debug.Log($"🏆 Logro desbloqueado: {logro.descripcion}");
            }
        }
    }

    void GuardarLogro(string descripcion)
    {
        string usuario = PlayerPrefs.GetString("usuario_seleccionado", "");

        if (!string.IsNullOrEmpty(usuario))
        {
            StartCoroutine(LogrosAPI.EnviarLogro(usuario, descripcion));
        }
    }

    void MostrarLogroUI(string texto)
    {
        // Aquí puedes implementar una animación tipo notificación estilo "Steam"
        Debug.Log("Mostrar en pantalla: " + texto);
    }
}
