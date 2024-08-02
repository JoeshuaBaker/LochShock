using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour
{
    public Animator animator;
    public ParticleSystem dangerZonePS;
    public CircleCollider2D dangerZoneCollider;
    public ContactFilter2D hitFilter;
    public List<Collider2D> hitBuffer;
    public bool dangerZonePSUnactive;
    public bool safeOnPlayer;
    public float delay;
    public float damageLocal;
    public float scale;
    public float particleDensity;
    public bool damageDealt;
    public bool willDealDamage;

   public void Setup( float damage , float delay , Vector3 position , bool dealsDamage )
    {
        animator.SetBool("skip", false);
        animator.Play(0);

        damageDealt = false;

        willDealDamage = dealsDamage;

        this.gameObject.SetActive(true);

        hitBuffer = new List<Collider2D>();
        hitFilter = new ContactFilter2D
        {
            useTriggers = false,
            layerMask = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Player"),
            useLayerMask = true
        };

        this.transform.position = position;
        this.delay = delay;
        this.damageLocal = damage;
        //this.scale = scale;
        //this.safeOnPlayer = safeOnPlayer;
        //this.dangerZonePSUnactive = dangerZonePSUnactive;
        //this.particleDensity = particleDensity;

        var dzPS = dangerZonePS.main;

        dzPS.startDelay = delay;

        if (delay == 0f)
        {
            animator.SetBool("skip", true);
        }

    }

    void Update()
    {
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

        if (willDealDamage)
        {

            if (animState.IsName("DangerZoneFinish") && damageDealt == false)
            {

                damageDealt = true;

                Physics2D.OverlapCollider(dangerZoneCollider, hitFilter, hitBuffer);

                foreach (Collider2D Collider in hitBuffer)
                {
                    Enemy enemy = Collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damageLocal);
                        continue;
                    }
                    Player player = Collider.GetComponent<Player>();
                    if (player != null)
                    {
                        player.TakeDamageFromEnemy(-1);
                    }
                }

            }
        }

        if (animState.IsName("DangerZoneFinish") && animState.normalizedTime >= 1)
        {
            this.gameObject.SetActive(false);
        }

    }
}
