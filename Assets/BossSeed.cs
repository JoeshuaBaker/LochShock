using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSeed : MonoBehaviour
{
    public Animator animatorSeed;
    public Animator animatorWarning;
    public ParticleSystem thornPSHard;
    public ParticleSystem thornPSHardSmall;
    public ParticleSystem thornPSBlur;
    public ParticleSystem thornPSDrift;

    public BezierCurve curve;
    public BezierPoint p0;
    public Vector3 p0Pos;
    public BezierPoint p1;
    public Vector3 p1Pos;
    public float length;
    public float bossSpeed = 1f;
    public float frameLengthOnCurve;
    public float bossLengthOnCurve;
    public float bossPercentDownCurve = 0f;

    public float bossSpawnX = 0f;
    public float bossMaxHP;

    public Player player;
    public World world;

    public bool setUp;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.activePlayer;
        world = World.activeWorld;

    }

    // Update is called once per frame
    void Update()
    {
        if (world.paused)
        {
            return;
        }

        if (player.transform.position.x >= bossSpawnX)
        {
            UpdateBossPos();
        }
    }

    public void UpdateBossPos()
    {
        if (!setUp)
        {
            Tile tile = world.RandomPathTileAtXPosition(2f);
            p0Pos = tile.transform.position;
            this.transform.position = p0Pos;
            p0.transform.position = p0Pos;


            Tile tile2 = world.RandomPathTileAtXPosition(27f);
            p1Pos = tile2.transform.position;
            p1.transform.position = p1Pos;

            setUp = true;
        }

        if (bossPercentDownCurve >= 1f)
        {
            bossLengthOnCurve = 0f;
            bossPercentDownCurve = 0f;

            p0Pos = p1Pos;

            float newX = p1Pos.x + 25f;

            Tile tile = world.RandomPathTileAtXPosition(newX);

            p1Pos = tile.transform.position;

        }

        if (bossPercentDownCurve < 1f && p0Pos != Vector3.zero)
        {
            Vector3 disToPlayer = this.transform.position - player.transform.position;
        
            if(disToPlayer.magnitude >= 25f)
            {
                return;
            }
            p0.transform.position = p0Pos;
            p1.transform.position = p1Pos;
            length = BezierCurve.ApproximateLength(p1, p0, 100);

            frameLengthOnCurve = (bossSpeed * Time.deltaTime *60f);

            bossLengthOnCurve = bossLengthOnCurve + frameLengthOnCurve;

            bossPercentDownCurve = 1f / (length / bossLengthOnCurve);

            this.transform.position = BezierCurve.GetPoint(p1, p0, 1f - bossPercentDownCurve);
        }
    }
}
