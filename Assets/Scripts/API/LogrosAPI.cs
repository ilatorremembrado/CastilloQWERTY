using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;

public class LogrosAPI
{
    [System.Serializable]
    public class LogroRequest
    {
        public string nombre;
        public string descripcion;
    }

    public static IEnumerator EnviarLogro(string nombre, string descripcion)
    {
        var datos = new LogroRequest { nombre = nombre, descripcion = descripcion };
        string json = JsonUtility.ToJson(datos);

        UnityWebRequest req = new UnityWebRequest($"{ApiConfig.BASE_URL}guardar_logro.php", "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("🏆 Logro enviado correctamente");
        }
        else
        {
            Debug.LogError("❌ Error al enviar logro: " + req.error);
        }
    }
}
