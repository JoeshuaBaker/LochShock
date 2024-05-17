using System.Collections;
using System.Collections.Generic;
using BulletHell;
using UnityEngine;

public class Urchin : Enemy
{
    //references
    public Player player;
    public Animator animator;

    //public properties
    public float speed = 1f;

    //internal state vars
    bool dying = false;
    public override void Start()
    {
        base.Start();
        player = Player.activePlayer;
        animator ??= GetComponent<Animator>();
    }

    public override int EnemyId()
    {
        return 1;
    }

    public void Update()
    {
        if (dying)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
            if (animState.IsName("UrchinDie") && animState.normalizedTime >= 1f)
            {
                this.gameObject.SetActive(false);
            }
            return;
        }

        if (HitFreeze())
        {
            return;
        }

        Vector3 directionToPlayer = player.transform.position - this.transform.position;

        if(directionToPlayer.magnitude > 30f)
        {
            this.gameObject.SetActive(false);
        }
        rb.velocity = new Vector2(directionToPlayer.x, directionToPlayer.y).normalized * speed;
    }

    public override void Die()
    {
        dying = true;
        animator.SetBool("Die", true);
        rb.simulated = false;
    }

    public override void Reset()
    {
        base.Reset();

        dying = false;
        animator.SetBool("Die", false);
        rb.simulated = true;
    }
}
