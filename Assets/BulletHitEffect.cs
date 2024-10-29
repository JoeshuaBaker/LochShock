using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;

public class BulletHitEffect : MonoBehaviour
{
    public ParticleSystem hitPS;
    public ParticleSystem bloodPS;
    public ParticleSystem bloodPSZone;
    public ParticleSystem treeDestroyPS;
    public ParticleSystem.MinMaxCurve bloodMin;
    public ParticleSystem.MinMaxCurve bloodMax;

    public float bloodAngle;


    public void EmitBulletHit(ProjectileData projectile, RaycastHit2D hitInfo , Vector3 enemyPos ,bool isKilled)
    {
        //if (!projectile.emitBulletHitParticle)
        //    return;

        var shapeMod = hitPS.shape;

        if (!isKilled)
        {
            this.transform.position = projectile.Position;
        }
        else
        {
            this.transform.position = enemyPos;
        }

        var direction = projectile.Velocity.normalized;

        //this.transform.position = hitInfo.transform.position;
        //var direction = new Vector2 (hitInfo.collider.gameObject.transform.position.x , hitInfo.collider.gameObject.transform.position.y) - hitInfo.point;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        shapeMod.arc = 140;
        hitPS.Emit(Random.Range(15, 35));
        shapeMod.arc = 360;
        hitPS.Emit(Random.Range(3, 8));

        var bloodMain = bloodPS.main;
        var bloodSize = bloodPS.main.startSize;
        var bloodZMain = bloodPSZone.main;
        var bloodZSize = bloodPSZone.main.startSize;

        bloodAngle = this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

        bloodMain.startRotation = bloodAngle;
        bloodZMain.startRotation = bloodAngle;

        if (isKilled)
        {
            bloodZMain.startSize = bloodMax;
            bloodPSZone.Emit(1);
        }
        else
        {
            bloodMain.startSize = bloodMin;
            bloodPS.Emit(1);
        }


    }

    public void EmitZoneHit(Vector3 pos, Vector3 rot, bool addHitEffect = false, float offsetAngle = 0f)
    {
        var shapeMod = hitPS.shape;

        this.transform.position = pos;
        var direction = new Vector2(rot.x, rot.y).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        this.transform.Rotate(0f, 0f, offsetAngle);

        if (addHitEffect)
        {
            shapeMod.arc = 140;
            hitPS.Emit(Random.Range(15, 35));
            shapeMod.arc = 360;
            hitPS.Emit(Random.Range(3, 8));
        }
    
        var bloodMain = bloodPSZone.main;
        var bloodSize = bloodPSZone.main.startSize;

        bloodAngle = this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

        bloodMain.startRotation = bloodAngle;

        bloodMain.startSize = bloodMax;

        bloodPSZone.Emit(1);
    }

    public void EmitTreeHit(Vector3 pos, Vector3 rot, float offsetAngle = 0f)
    {
        this.transform.position = pos;
        var direction = new Vector2(rot.x, rot.y).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        treeDestroyPS.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        this.transform.Rotate(0f, 0f, offsetAngle);

        var shapeMod = treeDestroyPS.shape;

        treeDestroyPS.Emit(8);
    }

}
