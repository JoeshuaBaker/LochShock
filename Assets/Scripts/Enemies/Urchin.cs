using System.Collections;
using System.Collections.Generic;
using BulletHell;
using UnityEngine;

public class Urchin : BasicEnemy
{
    public override void DeathAnimationBegin()
    {
        base.DeathAnimationBegin();

        //Audio Section
        //AkSoundEngine.PostEvent("EnemyDie", this.gameObject);
    }
}
