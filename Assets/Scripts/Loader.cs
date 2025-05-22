// mira que GameManager esté creado en la jerarquía, si no está lo instancia
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject gameManager;

    private void Awake()
    {
        //accedo al singleton
        if(GameManager.instance == null)
        {
            Instantiate(gameManager);
        }
    }
}
