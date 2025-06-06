using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public static class ApiConfig
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD          
    public const string BASE_URL = "http://localhost/castillo_qwerty_api/";   // IP o URL temporal para unity
#else                                          
    public const string BASE_URL = "https://castilloqwerty.oo.gd/";      // Dominio ya propagado
#endif
}

public class LoginAPI : MonoBehaviour
{
    [System.Serializable]
    public class LoginData
    {
        public string nombre;
        public string password;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public bool success;
        public string message;
    }

    void Start()
    {
        StartCoroutine(EnviarLogin("isabel", "123456"));
    }

    IEnumerator EnviarLogin(string nombre, string clave)
    {
        LoginData data = new LoginData { nombre = nombre, password = clave };
        string json = JsonUtility.ToJson(data);

        UnityWebRequest req = new UnityWebRequest($"{ApiConfig.BASE_URL}login.php", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error de red: " + req.error);
        }
        else
        {
            Debug.Log("Respuesta: " + req.downloadHandler.text);
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(req.downloadHandler.text);
            if (response.success)
                Debug.Log("Login exitoso");
            else
                Debug.Log("Fallo en login: " + response.message);
        }
    }
}
