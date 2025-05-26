using System.Collections.Generic;
using UnityEngine;

public class TypingManager : MonoBehaviour
{
    public static TypingManager instance;

    public TypingEnemy currentEnemy;
    private List<TypingEnemy> allEnemies = new List<TypingEnemy>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (GameManager.instance.doingSetup || currentEnemy == null) return;

        foreach (char c in Input.inputString)
        {
            if (char.IsLetter(c))
            {
                Debug.Log("Letra presionada: " + c);
                currentEnemy.HandleInput(char.ToLower(c));
            }
        }
    }

    public void RegisterEnemy(TypingEnemy enemy)
    {
        allEnemies.Add(enemy);

        if (currentEnemy == null)
        {
            currentEnemy = enemy;
            enemy.isActive = true;
            Debug.Log("Enemigo ACTIVADO: " + enemy.name);
        }
        else
        {
            enemy.isActive = false;
            Debug.Log("Enemigo EN ESPERA: " + enemy.name);
        }
    }

    public void SelectNextEnemy()
    {
        allEnemies.Remove(currentEnemy);
        currentEnemy = null;

        if (allEnemies.Count > 0)
        {
            currentEnemy = allEnemies[0];
            currentEnemy.isActive = true;
        }
    }
}
