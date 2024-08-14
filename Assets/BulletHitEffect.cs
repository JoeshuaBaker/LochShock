using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;

public class BulletHitEffect : MonoBehaviour
{
    public ParticleSystem hitPS;
    public Player player;
    public float timer=0.2f;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.activePlayer;
    }

    // Update is called once per frame
    void Update()
    {

        timer = timer - Time.deltaTime;

        var shapeMod = hitPS.shape;

        var pp = player.transform.position;
        var tp = this.transform.position;
        // shapeMod.arc = 360;

        //shapeMod.position = Vector3.zero;


        //var param = new ParticleSystem.EmitParams();
        //var param2 = new ParticleSystem.EmitParams();

        //param.position = new Vector3(pp.x - 1f, pp.y, 0f);

        //// param.velocity = new Vector3(100f, 100f, 0f);


        //param.applyShapeToPosition = true;
        //if (timer < 0)
        //{
        //    hitPS.Emit(param, Random.Range(15, 35));
        //    timer = 0.2f;
        //}

        tp = pp;

        if(timer < 0)
        {

            this.transform.position = Vector3.zero;

            shapeMod.arc = 140;
            hitPS.Emit(Random.Range(15, 35));
            shapeMod.arc = 360;
            hitPS.Emit(Random.Range(3, 8));

            this.transform.position = new Vector3(10f, 0f, 0f);

            shapeMod.arc = 140;
            hitPS.Emit(Random.Range(15, 35));
            shapeMod.arc = 360;
            hitPS.Emit(Random.Range(3, 8));

            timer = 0.2f;
        }
        
     
        
        
    }

    public void EmitBulletHit(List<ProjectileData> collisions)
    {
        var shapeMod = hitPS.shape;
        var tp = this.transform.position;
        var tr = this.transform.rotation;

        int hits = collisions.Count;

        for (int i = 0; i < hits; i++)
        {
            tp = collisions[i].Position;
        
            Vector2 dir = collisions[i].Velocity.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            tr = Quaternion.AngleAxis(angle, Vector3.forward);

            shapeMod.arc = 140;
            hitPS.Emit(Random.Range(15, 35));
            shapeMod.arc = 360;
            hitPS.Emit(Random.Range(3, 8));
        }

    }
}
