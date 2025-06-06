using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Enemy))]
public class TypingEnemy : MonoBehaviour
{
    public string word;
    public TextMeshPro textMesh;
    public int damagePerLetter = 1;
    public bool isActive = false;

    private int currentLetterIndex = 0;
    private Enemy enemy;
    private Player player;

    IEnumerator Start()
    {
        enemy = GetComponent<Enemy>();
        
        // Esperamos a que el TypingManager esté inicializado y el GameManager termine el setup
        yield return new WaitUntil(() => TypingManager.instance != null && !GameManager.instance.doingSetup);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (player == null)
        {
            Debug.LogError("❌ No se encontró el jugador en la escena.");
            yield break;
        }

        // Obtener dificultad desde GameManager o PlayerPrefs
        string dificultad = GameManager.instance.GetDificultadSeleccionada();

        // Elegir tipo según el nivel
        int nivel = GameManager.instance.GetLevel();
        string tipo = nivel < 5 ? "letra" :
                      nivel < 10 ? "palabra" :
                      new[] { "letra", "palabra", "frase" }[Random.Range(0, 3)];

        Debug.Log($"📥 Solicitando cadena | Dificultad: {dificultad}, Tipo: {tipo}");

        // Obtener palabra de base de datos
        bool palabraRecibida = false;
        string palabraObtenida = "ERROR";

        yield return WordGenerator.GetRandomWord(dificultad, tipo, (cadena) =>
        {
            palabraObtenida = cadena;
            palabraRecibida = true;
        },
        (error) =>
        {
            Debug.LogWarning($"⚠️ Error al obtener palabra: {error}");
            palabraRecibida = true;
        });

        // Esperar hasta recibir la palabra
        yield return new WaitUntil(() => palabraRecibida);

        word = palabraObtenida;
        textMesh.text = word;

        TypingManager.instance.RegisterEnemy(this);
        UpdateDisplayedText();
    }

    public void HandleInput(char input)
    {
        if (word[currentLetterIndex] == input)
        {
            currentLetterIndex++;
            enemy.ReceiveDamage(damagePerLetter);
            SoundManager.instance.PlaySound(SoundManager.instance.letterCorrectSound);
            TypingManager.instance.ContarLetraCorrecta();
            Debug.Log("✅ Letra correcta");

            UpdateDisplayedText();

            if (currentLetterIndex >= word.Length)
            {
                TypingManager.instance.ContarPalabraCompletada();
                Debug.Log("🏁 Palabra completada");
                StartCoroutine(Die());
            }         
        }
        else
        {
            TypingManager.instance.ContarLetraIncorrecta();
    Debug.Log("❌ Letra incorrecta");
    
    player.LoseLife(enemy.playerDamage);
    enemy.TriggerAttackAnimation();
    currentLetterIndex = 0;
    UpdateDisplayedText();
        }
    }

    void UpdateDisplayedText()
    {
        string correct = "<color=green>" + word.Substring(0, currentLetterIndex) + "</color>";
        string rest = word.Substring(currentLetterIndex);
        textMesh.text = correct + rest;
    }

    IEnumerator Die()
    {
        enemy.TriggerDeathAnimation();
        SoundManager.instance.PlaySound(SoundManager.instance.deathEnemySound);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
        TypingManager.instance.SelectNextEnemy();
    }

    void LateUpdate()
    {
        if (textMesh != null)
        {
            Vector3 scale = textMesh.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(transform.localScale.x);
            textMesh.transform.localScale = scale;
        }
    }
}
