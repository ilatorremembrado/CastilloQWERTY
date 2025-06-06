using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BonusSpawner : MonoBehaviour
{
    public GameObject bonusPrefab;
    public float interval = 10f;

    private GameObject currentBonus;
    private List<Vector2> availablePositions = new List<Vector2>();

    void Start()
    {
        StartCoroutine(BonusLoop());
    }

    IEnumerator BonusLoop()
    {
        while (true)
        {
            if (currentBonus == null && !GameManager.instance.doingSetup)
            {
                SpawnBonus();
            }

            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnBonus()
    {
        Vector2 position = GetValidBonusPosition();

        if (position != Vector2.zero)
        {
            currentBonus = Instantiate(bonusPrefab, position, Quaternion.identity);
        }
    }

    Vector2 GetValidBonusPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        BoardManager board = GameManager.instance.boardScript;
        int maxTries = 100;

        for (int i = 0; i < maxTries; i++)
        {
            float x = Random.Range(1f, board.columns - 2); // Evita muros
            float y = Random.Range(1f, board.rows - 2);
            Vector2 randomPos = new Vector2(Mathf.Round(x), Mathf.Round(y));

            bool tooCloseToPlayer = Vector2.Distance(player.transform.position, randomPos) < 1f;
            bool tooCloseToEnemy = false;

            foreach (GameObject enemy in enemies)
            {
                if (Vector2.Distance(enemy.transform.position, randomPos) < 1f)
                {
            }

            if (!tooCloseToPlayer && !tooCloseToEnemy)
                return randomPos;
        }

                tooCloseToEnemy = true;
                    break;
                }
        return Vector2.zero;
    }

}
