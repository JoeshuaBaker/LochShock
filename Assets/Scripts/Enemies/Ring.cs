using System.Collections;
using System.Collections.Generic;
using BulletHell;
using UnityEngine;

public class Ring : BasicEnemy
{
    public override int EnemyId()
    {
        return 3;
    }

    public override void DeathAnimationBegin()
    {
        base.DeathAnimationBegin();

        //Audio Section
        AkSoundEngine.PostEvent("PlayTestTone", this.gameObject);
    }
}
