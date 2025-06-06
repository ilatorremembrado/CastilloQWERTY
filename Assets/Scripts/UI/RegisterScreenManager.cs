using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System.Collections;

public class RegisterScreenManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmInput;
    public TMP_Text errorText;
    public Button registerButton;
    public Button cancelButton;

    public GameObject registerPanel;
    public GameObject userListPanel;
    public UserSelectionManager userSelector;

    [System.Serializable]
    public class RegisterRequest { public string nombre; public string password; }
    [System.Serializable]
    public class RegisterResponse { public bool success; public string message; }

    void Start()
    {
        registerButton.onClick.AddListener(EnviarRegistro);
        cancelButton.onClick.AddListener(() =>
        {
            registerPanel.SetActive(false);
            userListPanel.SetActive(true);
            usernameInput.text = "";
            passwordInput.text = "";
            confirmInput.text = "";
            errorText.text = "";
        });
        errorText.text = "";
    }

    void EnviarRegistro()
    {
        string usuario = usernameInput.text;
        string clave = passwordInput.text;
        string confirm = confirmInput.text;

        if (usuario.Length < 3)
        {
            errorText.text = "Nombre mínimo 3 letras.";
            return;
        }

        if (clave.Length < 6)
        {
            errorText.text = "Contraseña mínima 6 caracteres.";
            return;
        }

        if (clave != confirm)
        {
            errorText.text = "Las contraseñas no coinciden.";
            return;
        }

        StartCoroutine(RegistroCoroutine(usuario, clave));
    }

    IEnumerator RegistroCoroutine(string nombre, string clave)
    {
        RegisterRequest data = new RegisterRequest { nombre = nombre, password = clave };
        string json = JsonUtility.ToJson(data);

        UnityWebRequest req = new UnityWebRequest($"{ApiConfig.BASE_URL}register.php", "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            errorText.text = "Error de red: " + req.error;
        }
        else
        {
            RegisterResponse res = JsonUtility.FromJson<RegisterResponse>(req.downloadHandler.text);
            if (res.success)
            {
                registerPanel.SetActive(false);
                userListPanel.SetActive(true);
                userSelector.ReloadUsers();

                errorText.text = "";
            }
            else
            {
                errorText.text = res.message;
            }
        }
    }
}
