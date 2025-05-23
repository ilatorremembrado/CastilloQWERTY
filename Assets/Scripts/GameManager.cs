// Gestionar Juego
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// TODO: foodText.text = "+" + pointsPerWord + " Food: " + foodText; en un texto con variables para no repetir codigo
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float turnDelay = 0.1f;//tiempo a esperar entre movimientos (duración de un turno)
    public float levelStartDelay = 2f;

    // Decirle a BoardManager que genere un nivel nuevo
    public BoardManager boardScript;

    public int playerPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    private List<Enemy> enemies = new List<Enemy>();
    private bool enemiesMoving;

//los objetos referentes a la pantalla de información previa a cada partida se crean y se destruyen en cada partida para cambiar los siguientes valores
    private int level = 3;
    private GameObject levelImage;
    private Text levelText;
    public bool doingSetup;

    private void Awake()
    {
        //singleton: permite acceder al gamemanager desde cualquier script usando la variable instance
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }
        else if (GameManager.instance != this)
        {
            Destroy(gameObject);
        }

        //para que el gamemanager no se destruya al generar otra escena o recargar la misma
        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
    }

    void InitGame()
    {
        //prepara pantalla prejuego
        doingSetup = true; //para que el jugador no se pueda mover mientras se prepara la escena
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);

        //prepara escena juego
        enemies.Clear();
        boardScript.SetupScene(level);

        //en (levelStartDelay) segundos se genera el método (HideLevelImage)
        Invoke("HideLevelImage", levelStartDelay);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        enabled = false;
    }

    //corrutina que se encarga de mover a los enemigos por turnos
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        playersTurn = true;
        enemiesMoving = false;
    }

    private void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup) return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        level++;
        InitGame();
    }

}
