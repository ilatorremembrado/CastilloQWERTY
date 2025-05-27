using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine.SceneManagement;

public class LoginScreenManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject loginPanel;
    public GameObject userListPanel;
    public GameObject confirmDeletePanel;

    [Header("Login UI")]
    public TMP_Text textIntroducePassword;
    public TMP_InputField passwordField;
    public TMP_Text errorText;
    public Button loginButton;
    public Button cancelButton;
    public Button deleteUserButton;

    [Header("Confirm Delete UI")]
    public TMP_Text confirmDeleteText;
    public Button confirmDeleteAcceptButton;
    public Button confirmDeleteCancelButton;

    [Header("Referencias externas")]
    public UserSelectionManager userSelector;

    private string currentUser = "";
    private bool waitingForDeleteConfirmation = false;

    [System.Serializable]
    public class LoginRequest { public string nombre; public string password; }
    
    [System.Serializable]
    public class LoginResponse { public bool success; public string message; }

    [System.Serializable]
    public class DeleteUserRequest
    {
        public string nombre;

        public DeleteUserRequest(string nombre)
        {
            this.nombre = nombre;
        }
    }


    void Start()
    {
        loginPanel.SetActive(false);
        userListPanel.SetActive(true);
        confirmDeletePanel.SetActive(false);

        loginButton.onClick.AddListener(VerificarLogin);
        deleteUserButton.onClick.AddListener(VerificarBorrado);
        cancelButton.onClick.AddListener(CancelarLogin);
        confirmDeleteAcceptButton.onClick.AddListener(BorrarUsuario);
        confirmDeleteCancelButton.onClick.AddListener(() =>
        {
            confirmDeletePanel.SetActive(false);
            waitingForDeleteConfirmation = false;
        });
    }

    public void OpenLoginPanel(string nombre)
    {
        currentUser = nombre;
        PlayerPrefs.SetString("usuario_seleccionado", nombre);
        textIntroducePassword.text = $"Introduce la contraseña de {nombre}";

        passwordField.text = "";
        errorText.text = "";
        loginPanel.SetActive(true);
        userListPanel.SetActive(false);
    }

    void VerificarLogin()
    {
        string password = passwordField.text;
        if (string.IsNullOrEmpty(currentUser) || string.IsNullOrEmpty(password))
        {
            errorText.text = "Por favor, introduce la contraseña.";
            return;
        }

        StartCoroutine(ValidatePassword(currentUser, password, accesoJuego: true));
    }

    void VerificarBorrado()
    {
        string password = passwordField.text;
        if (string.IsNullOrEmpty(currentUser) || string.IsNullOrEmpty(password))
        {
            errorText.text = "Introduce tu contraseña para borrar.";
            return;
        }

        waitingForDeleteConfirmation = true;
        StartCoroutine(ValidatePassword(currentUser, password, accesoJuego: false));
    }

    IEnumerator ValidatePassword(string nombre, string password, bool accesoJuego)
    {
        LoginRequest data = new LoginRequest { nombre = nombre, password = password };
        string json = JsonUtility.ToJson(data);

        UnityWebRequest req = new UnityWebRequest("http://localhost/castillo_qwerty_api/login.php", "POST");
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
            LoginResponse res = JsonUtility.FromJson<LoginResponse>(req.downloadHandler.text);
            if (res.success)
            {
                errorText.text = "";

                if (accesoJuego)
                {
                    SceneManager.LoadScene("MenuPrincipal");
                }
                else if (waitingForDeleteConfirmation)
                {
                    confirmDeleteText.text = $"¿Seguro que quieres borrar la partida de {currentUser}?";
                    confirmDeletePanel.SetActive(true);
                }
            }
            else
            {
                errorText.text = "Contraseña incorrecta.";
            }
        }
    }

    void BorrarUsuario()
    {
        StartCoroutine(BorrarUsuarioCoroutine(currentUser));
    }

    IEnumerator BorrarUsuarioCoroutine(string nombre)
    {
        string json = JsonUtility.ToJson(new DeleteUserRequest(nombre));

        UnityWebRequest req = new UnityWebRequest("http://localhost/castillo_qwerty_api/delete_user.php", "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            errorText.text = "Error al borrar: " + req.error;
        }
        else
        {
            string respuesta = req.downloadHandler.text;

            confirmDeletePanel.SetActive(false);
            loginPanel.SetActive(false);
            userListPanel.SetActive(true);

            if (userSelector != null)
                userSelector.ReloadUsers();
        }
    }

    void CancelarLogin()
    {
        loginPanel.SetActive(false);
        userListPanel.SetActive(true);
        passwordField.text = "";
        errorText.text = "";
    }
}
