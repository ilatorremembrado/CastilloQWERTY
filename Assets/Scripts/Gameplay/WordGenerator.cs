using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public static class WordGenerator
{
     private const string URL = ApiConfig.BASE_URL + "get_cadena.php";
    // Llamada asíncrona desde TypingEnemy
    public static IEnumerator GetRandomWord(string dificultad, string tipo, Action<string> onSuccess, Action<string> onError = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("dificultad", dificultad);
        form.AddField("tipo", tipo);

    Debug.Log($"📤 Enviando a servidor -> Dificultad: {dificultad}, Tipo: {tipo}");

        UnityWebRequest www = UnityWebRequest.Post(URL, form);
        yield return www.SendWebRequest();

Debug.Log($"📥 Código respuesta HTTP: {www.responseCode}");


        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Error de red: " + www.error);
            onError?.Invoke("Error de red");
        }
        else
        {
            try
            {
                string json = www.downloadHandler.text;
                Debug.Log("📥 Respuesta del servidor: " + json);
                var data = JsonUtility.FromJson<RespuestaCadena>(json);
                Debug.Log("✅ JSON parseado: " + data.texto);

                if (data.success)
                {
                    onSuccess?.Invoke(data.texto);
                }
                else
                {
                    Debug.LogWarning("⚠️ Servidor respondió con error: " + data.message);
                    onError?.Invoke(data.message);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("❌ Error al procesar JSON: " + ex.Message);
                onError?.Invoke("Error de formato en JSON");
            }
        }
    }

    [Serializable]
    private class RespuestaCadena
    {
        public bool success;
        public string texto;
        public string message;
    }
}