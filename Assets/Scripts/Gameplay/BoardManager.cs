//Clase para gestionar cada nivel
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
	//area en la que se puede mover el personaje
    public int columns = 8;
    public int rows = 8;

    public GameObject[] floorTiles, outerWallTiles, enemyTiles; //losetas y enemigos

    private Transform boardHolder;
    private List<Vector2> gridPositions = new List<Vector2>(); //posiciones donde pueden aparecer los objetos

    //aseguro que los elementos no se superpongan entre ellos
    void InitializeList()
    {
        gridPositions.Clear();
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector2(x, y));
            }
        }
    }

    Vector2 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector2 pos = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex); // eliminamos el índice de la posición para que otro objeto no la pueda ocupar
        return pos;
    }

// posicionar objeto en lugar aleatorio
    void LayoutObjectAtRandom(GameObject[] tileArray, int min, int max)
    {
        int objectCount = Random.Range(min, max + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector2 position = RandomPosition();
            GameObject tileChoice = GetRandomInArray(tileArray);
            GameObject instance = Instantiate(tileChoice, position, Quaternion.identity);
            instance.transform.SetParent(boardHolder);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();

        int enemyCount = Mathf.FloorToInt(Mathf.Log(level, 2f)) + 1;
        LayoutEnemiesAtEdges(enemyTiles, enemyCount);
    }

// Crea el escenario inicial (suelo y borde)
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x <= columns; x++)
        {
            for (int y = -1; y <= rows; y++)
            {
		// inicializamos con una loseta de suelo aleatoria de todas las que tenemos
                GameObject toInstantiate = GetRandomInArray(floorTiles);

                if (x == -1 || y == -1 || x == columns || y == rows) // posición de muro
                {
                    toInstantiate = GetRandomInArray(outerWallTiles);
                }

                GameObject instance = Instantiate(toInstantiate, new Vector2(x, y), Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    List<Vector2> GetEdgePositions()
    {
        List<Vector2> edgePositions = new List<Vector2>();

        // Añadir columnas horizontales (arriba y abajo)
        for (int x = 1; x < columns - 1; x++)
        {
            edgePositions.Add(new Vector2(x, 0));            // borde inferior
            edgePositions.Add(new Vector2(x, rows - 1));     // borde superior
        }

        // Añadir filas verticales (izquierda y derecha)
        for (int y = 1; y < rows - 1; y++)
        {
            edgePositions.Add(new Vector2(0, y));            // borde izquierdo
            edgePositions.Add(new Vector2(columns - 1, y));  // borde derecho
        }

        return edgePositions;
    }


    void LayoutEnemiesAtEdges(GameObject[] tileArray, int count)
    {
        List<Vector2> edgePositions = GetEdgePositions();

        for (int i = 0; i < count && edgePositions.Count > 0; i++)
        {
            int index = Random.Range(0, edgePositions.Count);
            Vector2 pos = edgePositions[index];
            edgePositions.RemoveAt(index);

            GameObject enemyPrefab = GetRandomInArray(tileArray);
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
            enemy.transform.SetParent(boardHolder);
            Debug.Log("Enemigo creado en: " + pos);

        }
    }

    GameObject GetRandomInArray(GameObject[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
}