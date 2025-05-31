using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Text;
using UnityEngine.UI;

public class FullStatsScreenManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject statsPanel;
    public TMP_Text partidasText;
    public TMP_Text palabrasText;
    public TMP_Text erroresText;
    public TMP_Text tiempoText;
    public TMP_Text nivelMaxText;
    public TMP_Text velocidadText;
    public TMP_Text precisionText;
    public TMP_Text errorText;
    public Button cerrarButton;

    [System.Serializable]
    public class StatsResponse
    {
        public bool success;
        public string message;
        public StatsData data;
    }

    [System.Serializable]
    public class StatsData
    {
        public int partidas;
        public int palabras_correctas;
        public int errores;
        public int tiempo_total;
        public int nivel_maximo;
        public float velocidad_media;
        public float precision_media;
    }

    [System.Serializable]
    public class StatsRequest
    {
        public string nombre;

        public StatsRequest(string nombre)
        {
            this.nombre = nombre;
        }
    }

    void Start()
    {
        statsPanel.SetActive(false);

        string usuario = PlayerPrefs.GetString("usuario_seleccionado", "");
        if (!string.IsNullOrEmpty(usuario))
        {
            StartCoroutine(CargarEstadisticas(usuario));
        }
        else
        {
            errorText.text = "❌ No se encontró el nombre de usuario.";
        }
    }

    public void MostrarEstadisticas()
    {
        string usuario = PlayerPrefs.GetString("usuario_seleccionado", "");
        Debug.Log("📥 MostrarEstadisticas: usuario encontrado = '" + usuario + "'");

        if (!string.IsNullOrEmpty(usuario))
        {
            statsPanel.SetActive(true);
            if (cerrarButton != null)
                cerrarButton.onClick.AddListener(CerrarEstadisticas);

            StartCoroutine(CargarEstadisticas(usuario));
        }
        else
        {
            errorText.text = "No se encontró el nombre de usuario.";
            Debug.LogError("❌ PlayerPrefs[usuario_seleccionado] está vacío.");
        }
    }   
    
    IEnumerator CargarEstadisticas(string nombre)
    {
        var json = JsonUtility.ToJson(new StatsRequest(nombre));

        Debug.Log("📡 Enviando JSON a get_full_stats.php: " + json);


        UnityWebRequest req = new UnityWebRequest("http://localhost/castillo_qwerty_api/get_full_stats.php", "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            errorText.text = "Error al conectar: " + req.error;
            yield break;
        }
        Debug.Log("📨 Respuesta recibida: " + req.downloadHandler.text);

        StatsResponse res = JsonUtility.FromJson<StatsResponse>(req.downloadHandler.text);
        if (!res.success)
        {
            errorText.text = "Error: " + res.message;
            yield break;
        }

        // Asignar a la UI con colores y emojis opcionales
        partidasText.text = "<b>Partidas jugadas:</b> " + res.data.partidas;
        palabrasText.text = "<b>Palabras correctas:</b> " + res.data.palabras_correctas;
        erroresText.text = "<b>Errores cometidos:</b> " + res.data.errores;
        tiempoText.text = "<b>Tiempo total jugado:</b> " + FormatearTiempo(res.data.tiempo_total);
        nivelMaxText.text = "<b>Nivel máximo alcanzado:</b> " + res.data.nivel_maximo;

        // Colores dinámicos
        string colorVel = GetColorPorVelocidad(res.data.velocidad_media);
        string colorPrec = GetColorPorPrecision(res.data.precision_media);

        // Velocidad
        velocidadText.text = $"<b>Velocidad media:</b> <color={colorVel}>{res.data.velocidad_media:F2} palabras/minuto</color>";

        // Precisión
        precisionText.text = $"<b>Precisión media:</b> <color={colorPrec}>{res.data.precision_media:F1} %</color>";


        errorText.text = "";
    }

    string GetColorPorVelocidad(float velocidad)
    {
        if (velocidad < 20f) return "red";
        if (velocidad < 40f) return "orange";
        if (velocidad < 60f) return "yellow";
        return "green";
    }

    string GetColorPorPrecision(float precision)
    {
        if (precision < 70f) return "red";
        if (precision < 85f) return "orange";
        if (precision < 95f) return "yellow";
        return "green";
    }

    string FormatearTiempo(int segundos)
    {
        int horas = segundos / 3600;
        int minutos = (segundos % 3600) / 60;
        int segs = segundos % 60;

        return $"{horas}h {minutos}m {segs}s";
    }
    
    public void CerrarEstadisticas()
    {
        statsPanel.SetActive(false);
    }
}
