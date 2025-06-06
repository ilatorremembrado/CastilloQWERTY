using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class RegisterAPI : MonoBehaviour
{
    [System.Serializable]
    public class RegisterData
    {
        public string nombre;
        public string password;
    }

    [System.Serializable]
    public class RegisterResponse
    {
        public bool success;
        public string message;
    }

    void Start()
    {
        StartCoroutine(RegistrarUsuario("isabel2", "123456"));
    }

    IEnumerator RegistrarUsuario(string nombre, string clave)
    {
        RegisterData data = new RegisterData { nombre = nombre, password = clave };
        string json = JsonUtility.ToJson(data);

        UnityWebRequest req = new UnityWebRequest($"{ApiConfig.BASE_URL}register.php", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Error de red: " + req.error);
        }
        else
        {
            Debug.Log("📦 Respuesta: " + req.downloadHandler.text);
            RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(req.downloadHandler.text);
            if (response.success)
                Debug.Log("✅ Registro exitoso: " + response.message);
            else
                Debug.Log("❌ Error en registro: " + response.message);
        }
    }
}
