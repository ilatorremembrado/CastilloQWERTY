// Gestionar cada nivel
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    //area en la que se puede mover el personaje
    public int columns = 8;
    public int rows = 8;

    public GameObject[] floorTiles, outerWallTiles, enemyTiles; // Losetas
    private Transform boardHolder; 

    //lista de posiciones donde pueden aparecer los objetos
    private List<Vector2> gridPositions = new List<Vector2>();

    // me aseguro de que los elementos no se superpongan entre ellos
    void InitializeList(){
        gridPositions.Clear();
        for (int i = 1; i < columns-1; i++)
        {
            for (int j = 1; j < rows-1; j++)
            {
                gridPositions.Add(new Vector2(i,j));
            }
        }
    }

    Vector2 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector2 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex); // eliminamos el índice de la posición para que otro objeto no la pueda ocupar
        return randomPosition;
    }

// posicionar objeto en lugar aleatorio
    void LayoutObjectAtRandom(GameObject[] tileArray, int min, int max)
    {
        int objectCount = Random.Range(min, max+1);
        for (int i = 0; i < objectCount; i++)
        {
            //posición aleatoria donde voy a colocar el primer objeto
            Vector2 randomPosition = RandomPosition();
            GameObject tileChoice = GetRandomInArray(tileArray);
            GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
            instance.transform.SetParent(boardHolder);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();
        int enemyCount = (int)Mathf.Log(level, 2) + 1;
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        //Instantiate(exit, new Vector2(columns-1, rows-1), Quaternion.identity);
        //si creo una salida la instancio aqui
    }

    // Crea el escenario inicial (suelo y borde)
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for(int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                // inicializamos con una loseta de suelo aleatoria de todas las que tenemos
                GameObject toInstantiate = GetRandomInArray(floorTiles);

                if(x == -1 || y==-1 || x==columns || y == rows) // posición de muro
                {
                    toInstantiate = GetRandomInArray(outerWallTiles);
                }

                GameObject instance = Instantiate(toInstantiate, new Vector2(x,y), Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    GameObject GetRandomInArray(GameObject[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
}
