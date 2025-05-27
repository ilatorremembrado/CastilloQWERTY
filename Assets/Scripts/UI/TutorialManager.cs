using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("Panel y componentes")]
    public GameObject tutorialPanel;
    public Image imageTutorial;
    public TMP_Text textTutorial;
    public Button buttonCerrar;
    public Button buttonSiguiente;

    [Header("Sprites del tutorial")]
    public Sprite sprite1;
    public Sprite sprite2;

    private bool mostrandoPrimeraParte = true;

    // Textos largos asignados desde el código por claridad
    private string texto1 = "¡Antes de empezar a teclear como un pro, siéntate bien! La espalda debe estar recta y apoyada al respaldo, con el cuello relajado y mirando al frente. Nada de encorvarse ni agachar la cabeza. Los hombros tienen que estar sueltos (no subidos como si hiciera frío), los codos doblados en ángulo recto y las muñecas alineadas con los antebrazos, sin doblarse hacia arriba o abajo. Las piernas deben formar un ángulo de 90° con los pies bien apoyados en el suelo. Ajusta la silla para que tus brazos estén cómodos sobre la mesa, coloca el teclado justo delante y el monitor a la altura de los ojos. Estar bien sentado no solo te hace jugar mejor, ¡también te evita dolores!";
    
    private string texto2 = "Ahora, manos al teclado. Coloca los dedos índice sobre las teclas “F” y “J” (tienen un pequeño relieve que se nota al tacto) y deja que el resto de dedos caigan naturalmente sobre las teclas de su fila: A, S, D para la izquierda y K, L, Ñ para la derecha. Los pulgares descansan sobre la barra espaciadora. Las muñecas deben mantenerse rectas y ligeramente elevadas, sin apoyarse sobre la mesa. Para escribir, solo debes mover los dedos, no las manos enteras. Con práctica, tus dedos sabrán llegar solos a cada tecla y escribirás sin mirar, ¡como un auténtico guerrero de la mecanografía!";

    void Start()
    {
        // Asegúrate de que está inactivo al inicio
        tutorialPanel.SetActive(false);

        // Asignar eventos a botones
        buttonCerrar.onClick.AddListener(CerrarTutorial);
        buttonSiguiente.onClick.AddListener(CambiarTutorial);

        // (Opcional) Si quieres que arranque activo para pruebas
        // MostrarTutorial();
    }

    public void MostrarTutorial()
    {
        tutorialPanel.SetActive(true);
        mostrandoPrimeraParte = true;
        ActualizarContenido();
    }

    void CerrarTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    void CambiarTutorial()
    {
        mostrandoPrimeraParte = !mostrandoPrimeraParte;
        ActualizarContenido();
    }

    void ActualizarContenido()
    {
        if (mostrandoPrimeraParte)
        {
            imageTutorial.sprite = sprite1;
            textTutorial.text = texto1;
        }
        else
        {
            imageTutorial.sprite = sprite2;
            textTutorial.text = texto2;
        }
    }
}
