using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;

public class UserSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class UsuarioResponse
    {
        public bool success;
        public string[] usuarios;
    }


    public GameObject userButtonPrefab;
    public Transform userListContainer;

    void Start()
    {
        StartCoroutine(CargarUsuarios());
    }

    IEnumerator CargarUsuarios()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost/castillo_qwerty_api/get_users.php");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al cargar usuarios: " + request.error);
            yield break;
        }

Debug.Log("✅ Respuesta JSON: " + request.downloadHandler.text);
        string json = request.downloadHandler.text;
        UsuarioResponse respuesta = JsonUtility.FromJson<UsuarioResponse>(json);

        if (!respuesta.success)
        {
            Debug.LogError("Respuesta de servidor fallida.");
            yield break;
        }

        Debug.Log("🧍 Usuarios recibidos: " + string.Join(", ", respuesta.usuarios));

        foreach (string nombre in respuesta.usuarios)
        {
            GameObject nuevoBoton = Instantiate(userButtonPrefab, userListContainer);
            nuevoBoton.GetComponentInChildren<TMP_Text>().text = nombre;
            nuevoBoton.GetComponent<Button>().onClick.AddListener(() => SeleccionarUsuario(nombre));
        }
    }

    void SeleccionarUsuario(string nombre)
    {
        Debug.Log("👤 Usuario seleccionado: " + nombre);

        // Puedes guardar este nombre en una variable global o GameManager
        PlayerPrefs.SetString("usuario_seleccionado", nombre);
        SceneManager.LoadScene("Juego");

Debug.Log("🌍 Cargando escena JUEGO...");
        // Opcional: ir a la pantalla de login para pedir la contraseña
        // SceneManager.LoadScene("PantallaLogin");
    }
}
