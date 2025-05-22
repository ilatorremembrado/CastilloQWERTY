using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int pointsPerWord = 0;
    public float restartLevelDelay = 1f;//tiempo a eseprar antes de recargar la siguiente escena
    public Text foodText;

    private Animator animator;
    private int life;

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        base.Awake();
    }

    protected override void Start()
    {
        life = GameManager.instance.playerPoints;
        foodText.text = "Food: " + life;
        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerPoints = life;
    }

    //si hemos llegado al final del nivel
    void CheckIfGameOver(){
        if (life <= 0 ) GameManager.instance.GameOver();
    }

    protected override void AttemptMove(int xDir, int yDir)
    {
        // cuando el jugador intente moverse decrementamos puntuación
        life--;
        foodText.text = "Food: " + life;
        animator.SetTrigger("playerRun");
        base.AttemptMove(xDir, yDir);
        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }

    protected override void OnCantMove(GameObject go)
    {
        /*Wall hitWall = go.GetComponent<Wall>();
        if(hitWall != null)
        {
            hitWall.DamageWall(wallDamage);
            animator.SetTrigger("playerChop");
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playersTurn || GameManager.instance.doingSetup) return;
        
        int horizontal = 0;
        int vertical = 0;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) horizontal = -1;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) horizontal = 1;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) vertical   = 1;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) vertical   = -1;

        //para que no se mueva en diagonal
        if (horizontal != 0) vertical = 0;

        // cambiar la dirección del asset
        if (horizontal > 0 )
        {
            Vector2 s = transform.localScale;
            s.x = Mathf.Abs(s.x);
            transform.localScale = s;        
        }
        else if (horizontal < 0)
        {
            Vector2 s = transform.localScale;
            s.x = -Mathf.Abs(s.x);
            transform.localScale = s;
        }
        
        if(horizontal != 0 || vertical != 0) AttemptMove(horizontal, vertical);
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoseLife(int loss)
    {
        life -= loss;
        foodText.text = "-" + loss + " Food: " + foodText;
        animator.SetTrigger("playerHurt");
        CheckIfGameOver();
    }

//metodo para futuras implementaciones con alimentos o recompensas
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Exit"))
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }else if (collision.CompareTag("Food"))
        {
            life += pointsPerWord;
            foodText.text = "+" + pointsPerWord + " Food: " + foodText;
            collision.gameObject.SetActive(false);
        }
        else if (collision.CompareTag("Soda"))
        {
            life += pointsPerWord;
            foodText.text = "+" + pointsPerWord + " Food: " + foodText;
            collision.gameObject.SetActive(false);
        }
    }
}
