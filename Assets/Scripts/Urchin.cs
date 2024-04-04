using System.Collections;
using System.Collections.Generic;
using BulletHell;
using UnityEngine;

public class Urchin : Enemy
{
    //references
    Player player;
    Animator animator;

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

    public void Update()
    {
        if (dying)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
            if (animState.IsName("UrchinDie") && animState.normalizedTime >= 1f)
            {
                Destroy(this.gameObject);
            }
            return;
        }

        if (HitFreeze())
        {
            return;
        }

        Vector3 directionToPlayer = player.transform.position - this.transform.position;
        rb.velocity = new Vector2(directionToPlayer.x, directionToPlayer.y).normalized * speed;
    }

    public override void Die()
    {
        dying = true;
        animator.SetBool("Die", true);
        rb.simulated = false;
    }
}
