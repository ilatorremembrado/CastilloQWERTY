using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public static class UserAPI
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

    [System.Serializable]
    public class RegisterResponse
    {
        public bool success;
        public string message;
    }

    public static IEnumerator EnviarLogin(string nombre, string password, System.Action<bool, string> callback)
    {
        LoginData data = new LoginData { nombre = nombre, password = password };
        string json = JsonUtility.ToJson(data);

        UnityWebRequest req = new UnityWebRequest("http://localhost/castillo_qwerty_api/login.php", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            callback(false, "Error de red: " + req.error);
        }
        else
        {
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(req.downloadHandler.text);
            callback(response.success, response.message);
        }
    }

    public static IEnumerator EnviarRegistro(string nombre, string password, System.Action<bool, string> callback)
    {
        LoginData data = new LoginData { nombre = nombre, password = password };
        string json = JsonUtility.ToJson(data);

        UnityWebRequest req = new UnityWebRequest("http://localhost/castillo_qwerty_api/register.php", "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            callback(false, "Error de red: " + req.error);
        }
        else
        {
            RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(req.downloadHandler.text);
            callback(response.success, response.message);
        }
    }
}
