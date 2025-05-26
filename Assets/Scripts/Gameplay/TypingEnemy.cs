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

        word = WordGenerator.GetRandomWord();
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
            UpdateDisplayedText();

            if (currentLetterIndex >= word.Length)
                StartCoroutine(Die());
        }
        else
        {
            player.LoseLife(10);
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
