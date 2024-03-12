using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;

public abstract class BulletCollidable : MonoBehaviour
{
    public abstract void ProcessCollision(ProjectileData projectile);
}
