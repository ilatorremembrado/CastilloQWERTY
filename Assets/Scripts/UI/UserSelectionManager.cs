using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class UserSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class UsuarioResponse
    {
        public bool success;
        public string[] usuarios;
    }

    [Header("Paneles")]
    public GameObject userListPanel;
    public GameObject loginPanel;
    public GameObject registerPanel;

    [Header("Referencias")]
    public GameObject userButtonPrefab;
    public Button newUserButton;
    public Transform userListContainer;
    public LoginScreenManager loginScreenManager; // ← Referencia directa

    void Start()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        userListPanel.SetActive(true);

        newUserButton.onClick.AddListener(() =>
        {
            userListPanel.SetActive(false);
            registerPanel.SetActive(true);
        });

        LoadUsers();
    }

    public void LoadUsers()
    {
        foreach (Transform child in userListContainer)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(CargarUsuarios());
    }

    IEnumerator CargarUsuarios()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost/castillo_qwerty_api/get_users.php");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            yield break;
        }

        string json = request.downloadHandler.text;

        UsuarioResponse respuesta = JsonUtility.FromJson<UsuarioResponse>(json);

        if (!respuesta.success || respuesta.usuarios == null)
        {
            yield break;
        }

        foreach (string nombre in respuesta.usuarios)
        {
            GameObject bttn = Instantiate(userButtonPrefab, userListContainer);
            bttn.GetComponentInChildren<TMP_Text>().text = nombre;

            bttn.GetComponent<Button>().onClick.AddListener(() =>
            {
                loginScreenManager.OpenLoginPanel(nombre); // ← uso directo
            });
        }
    }

    public void ReloadUsers()
    {
        LoadUsers();
    }
}
