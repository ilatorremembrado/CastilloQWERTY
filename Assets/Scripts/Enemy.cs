using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float stoppingDistance = 1f;
    public int playerDamage = 10;

    private Transform target;
    private Rigidbody2D rb2D;
    private Animator animator;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform; //asigno la referencia del jugador
// base.Start(); //mantiene el start de la clase principal
    }

    void Update()
    {
        // Esperar hasta que el GameManager haya terminado la pantalla de carga
        if (GameManager.instance.doingSetup) return;

        if (target == null) return;

        // 1) Calculamos distancia al jugador
        float dist = Vector2.Distance(transform.position, target.position);

        if (dist > stoppingDistance)
        {
            // 2) Solo mientras estemos fuera de stoppingDistance avanzamos
            Vector2 direction = (target.position - transform.position).normalized;
            rb2D.linearVelocity      = direction * moveSpeed; // <— usa velocity en vez de linearVelocity
            animator.SetTrigger("enemyMove");

            // Flip del sprite
            Vector3 scale = transform.localScale;
            scale.x = direction.x > 0 
                      ? Mathf.Abs(scale.x) 
                      : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else
        {
            // 3) Cuando llegamos a stoppingDistance, paramos
            rb2D.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // esto solo ocurre si colisionan físicamente,
            // y como ya no avanzamos hasta superponernos, 
            // solo pasarás a esta parte si el Player te empuja.
            Player hitPlayer = collision.gameObject.GetComponent<Player>();
            if (hitPlayer != null)
            {
                hitPlayer.LoseLife(playerDamage);
                animator.SetTrigger("enemyAttack1");
            }
        }
    }
}
