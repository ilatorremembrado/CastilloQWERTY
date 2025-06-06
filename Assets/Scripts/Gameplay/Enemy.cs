using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float stoppingDistance = 1f;
    public int playerDamage = 5;
    public float damageInterval = 3f;

    private Transform target;
    private Rigidbody2D rb2D;
    private Animator animator;

    private bool isTouchingPlayer = false;
    private Coroutine damageCoroutine;


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

        TypingEnemy typing = GetComponent<TypingEnemy>();
        if (typing != null && !typing.isActive)
        {
            rb2D.linearVelocity = Vector2.zero;
            return;
        }

        if (target == null) return;

        // 1) Calculamos distancia al jugador
        float dist = Vector2.Distance(transform.position, target.position);

        if (dist > stoppingDistance)
        {
            // 2) Solo mientras estemos fuera de stoppingDistance avanzamos
            Vector2 direction = (target.position - transform.position).normalized;
            rb2D.linearVelocity = direction * moveSpeed; // <— usa velocity en vez de linearVelocity
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

    // Cuando entra en contacto con el jugador
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DamageOverTime(other.GetComponent<Player>()));
            }
        }
    }

    // Cuando se aleja del jugador
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isTouchingPlayer = false;
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DamageOverTime(Player player)
    {
        while (isTouchingPlayer && player != null)
        {
            player.LoseLife(playerDamage);
            SoundManager.instance.PlaySound(SoundManager.instance.hurtPlayerSound);
            animator.SetTrigger("enemyAttack1");
            yield return new WaitForSeconds(damageInterval);
        }
    }
    
     public void ReceiveDamage(int amount)
    {
        animator.SetTrigger("enemyHurt");
    }

    public void TriggerAttackAnimation()
    {
        animator.SetTrigger("enemyAttack1");
    }

    public void TriggerDeathAnimation()
    {
        animator.SetTrigger("enemyDeath");
    }
}
