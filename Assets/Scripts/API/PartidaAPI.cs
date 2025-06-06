using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;

public static class PartidaAPI
{
    [Serializable]
    public class PartidaRequest
    {
        public string nombre;
        public int nivel;
        public int puntuacion;
        public float velocidad;
        public float precision;

        public int palabras;
        public int errores;
        public int duracion;
    }

    public static IEnumerator EnviarPartida(PartidaRequest datos, Action<bool, string> callback)
    {
        string json = JsonUtility.ToJson(datos);

        UnityWebRequest req = new UnityWebRequest($"{ApiConfig.BASE_URL}insert_partida.php", "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
            callback(false, req.error);
        else
            callback(true, req.downloadHandler.text);
    }
}
