using System.Collections;
using System.Collections.Generic;
using BulletHell;
using UnityEngine;

public class Ring : BasicEnemy
{

    public override void DeathAnimationBegin()
    {
        base.DeathAnimationBegin();

        //Audio Section
        PlayAudioOnEnemy("PlayEnemyDie", "BulletImpactSpeakerPan_LR");


    }
}
