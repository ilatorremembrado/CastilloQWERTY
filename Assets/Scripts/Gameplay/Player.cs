using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int pointsPerWord = 10;
    public float restartLevelDelay = 1f;
    public Text livesText;

    private Rigidbody2D rb2D;
    private Animator animator;
    private int life;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        life = GameManager.instance.playerPoints;
        UpdateLivesText();
    }

	// se llama al método una vez por frame
    void Update()
    {
        if (GameManager.instance.doingSetup) return;

        float moveX = 0, moveY = 0;

        if (Input.GetKey(KeyCode.LeftArrow)) moveX = -1;
        else if (Input.GetKey(KeyCode.RightArrow)) moveX = 1;

        if (Input.GetKey(KeyCode.UpArrow)) moveY = 1;
        else if (Input.GetKey(KeyCode.DownArrow)) moveY = -1;

        if (moveX != 0) moveY = 0;

        Vector2 movement = new Vector2(moveX, moveY).normalized;
        rb2D.linearVelocity = movement * moveSpeed;

        if (movement != Vector2.zero)
        {
            animator.SetTrigger("playerRun");

            Vector2 scale = transform.localScale;
            scale.x = moveX > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void OnDisable()
    {
        GameManager.instance.playerPoints = life;
    }

    private void UpdateLivesText()
    {
        livesText.text = "Vidas: " + life;
    }

	// comprueba si hemos llegado al final del nivel
    private void CheckIfGameOver()
    {
        if (life <= 0)
        {
            GameManager.instance.GameOver();
        }
    }

    public void LoseLife(int loss)
    {
        life -= loss;
        livesText.text = "Vidas: " + life;
        animator.SetTrigger("playerHurt");
        StartCoroutine(FlashDamage("-" + loss));
        CheckIfGameOver();
    }

    IEnumerator FlashDamage(string damageText)
    {
        livesText.text = damageText;
        yield return new WaitForSeconds(0.5f);
        livesText.text = "Vidas: " + life;
    }


    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

	// futuras implementaciones con alimentos o recompensas
    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Exit"))
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (collision.CompareTag("Food") || collision.CompareTag("Soda"))
        {
            life += pointsPerWord;
            livesText.text = "+" + pointsPerWord + " Food: " + life;
            collision.gameObject.SetActive(false);
        }
    }*/
}