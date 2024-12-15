using UnityEngine;

public class Eye : BasicEnemy
{
    public override void Update()
    {
        base.Update();
        animator.SetFloat("RandomTransition", Random.Range(0f, 1f));
    }

    public override void DeathAnimationBegin()
    {
        base.DeathAnimationBegin();

        //Audio Section
        PlayAudioOnEnemy("PlayEnemyDie", "BulletImpactSpeakerPan_LR");

    }

}