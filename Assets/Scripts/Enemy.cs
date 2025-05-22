using System;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove; //los enemigos se mueven un turno si y uno no

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        base.Awake();
    }

    protected override void Start()
    {
        //añado los enemigos a la lista de gamemanager al crearlos
        GameManager.instance.AddEnemyToList(this);
        target = GameObject.FindGameObjectWithTag("Player").transform; //asigno la referencia del jugador
        base.Start(); //mantiene el start de la clase principal
    }

    protected override void AttemptMove(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove(xDir, yDir);
        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        //si están en la misma columna el enemigo se mueve en vertical hacia donde está el jugador
        if(Math.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        //si no están en la misma columna se moverá en horizontal
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        animator.SetTrigger("enemyMove");
        AttemptMove(xDir, yDir);
    }

    protected override void OnCantMove(GameObject go)
    {
        Player hitPlayer = go.GetComponent<Player>();
        if(hitPlayer != null)
        {
            hitPlayer.LoseLife(playerDamage);
            animator.SetTrigger("enemyAttack1");
        }
    }
}
