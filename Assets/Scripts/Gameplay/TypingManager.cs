using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingManager : MonoBehaviour
{
    public static TypingManager instance;
    public TypingEnemy currentEnemy;
    public GameObject victoryPanel;

    public Text statsText;

    private List<TypingEnemy> allEnemies = new List<TypingEnemy>();

    private int letrasCorrectas = 0;
    private int letrasIncorrectas = 0;
    private int palabrasCompletadas = 0;
    private float tiempoInicio = 0f;

    public int PalabrasCompletadas => palabrasCompletadas;
    public int LetrasCorrectas => letrasCorrectas;
    public int LetrasIncorrectas => letrasIncorrectas;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (GameManager.instance.doingSetup || currentEnemy == null) return;

        if (tiempoInicio == 0f)
        tiempoInicio = Time.time; // Empezar a contar justo cuando empieza la partida

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
        else
        {
            Debug.Log("📊 Guardando estadísticas del nivel:");
Debug.Log($"Palabras: {palabrasCompletadas}");
Debug.Log($"Letras correctas: {letrasCorrectas}");
Debug.Log($"Letras incorrectas: {letrasIncorrectas}");
Debug.Log($"Precisión: {CalcularPrecision():F1}%");
Debug.Log($"Velocidad: {CalcularVelocidad():F2} ppm");
            GameManager.instance.statsUltimoNivel = new NivelStats
            {
                palabras = palabrasCompletadas,
                letrasCorrectas = letrasCorrectas,
                letrasIncorrectas = letrasIncorrectas,
                velocidad = CalcularVelocidad(),
                precision = CalcularPrecision()
            };
            
            // Acumular en statsPartida
            float duracionNivel = Time.time - tiempoInicio;
            GameManager.instance.statsPartida.Agregar(GameManager.instance.statsUltimoNivel, duracionNivel);

            // Reiniciar el tiempo para el siguiente nivel
            tiempoInicio = Time.time;

            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
                SoundManager.instance.PlaySound(SoundManager.instance.passLevelSound);

                float velocidad = CalcularVelocidad();
                float precision = CalcularPrecision();

                string resumen = "";
                resumen += $"<b>Palabras completadas: </b> {palabrasCompletadas}\n";
                resumen += $"<b>Letras correctas: </b> {letrasCorrectas}\n";
                resumen += $"<b>Letras incorrectas: </b> {letrasIncorrectas}\n";
                resumen += $"<b>Precisión: </b> {precision:F1}%\n";
                resumen += $"<b>Velocidad: </b> {velocidad:F2} ppm";

                if (statsText != null)
                    statsText.text = resumen;
                Debug.Log("✅ StatsText actualizado con el siguiente resumen:");
                Debug.Log(resumen);
            }

            // Inicia transición de nivel
            GameManager.instance.StartCoroutine(GameManager.instance.IrAlSiguienteNivel());
        }
    }

    public NivelStats ObtenerEstadisticasActuales()
    {
        float duracion = Time.time - tiempoInicio;
        return new NivelStats
        {
            palabras = palabrasCompletadas,
            letrasCorrectas = letrasCorrectas,
            letrasIncorrectas = letrasIncorrectas,
            velocidad = CalcularVelocidad(),
            precision = CalcularPrecision()
        };
    }

    public float ObtenerDuracion()
    {
        return Time.time - tiempoInicio;
    }
    
    // MÉTRICAS
    public void ContarLetraCorrecta() => letrasCorrectas++;
    public void ContarLetraIncorrecta() => letrasIncorrectas++;
    public void ContarPalabraCompletada() => palabrasCompletadas++;

    public float CalcularVelocidad()
    {
        float tiempoTotalMin = (Time.time - tiempoInicio) / 60f;
         Debug.Log($"🕒 Tiempo en minutos: {tiempoTotalMin:F2}");
        if (tiempoTotalMin <= 0.01f) return 0f;
        return palabrasCompletadas / tiempoTotalMin;
    }

    public float CalcularPrecision()
    {
        int total = letrasCorrectas + letrasIncorrectas;
        Debug.Log($"🎯 Letras correctas: {letrasCorrectas}, incorrectas: {letrasIncorrectas}, total: {total}");
        if (total == 0) return 100f;
        return (letrasCorrectas / (float)total) * 100f;
    }
}
