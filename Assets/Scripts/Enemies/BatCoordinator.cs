using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatCoordinator : EliteCoordinator
{
    public float minPathLength = 25f;
    public float randomPathBonusLength = 10f;
    [Range(0, 5)] public float flockSpeed = 1.75f;
    [Range(0, 5)] public float catchUpMultiplier = 2.5f;
    [Range(0, 30)] public float catchUpMaxDistance = 10f;
    public float numberOfBats = 26;
    public float swarmSize = 3f;
    public float turnPower = 25f;
    public BezierCurve pathCurve;
    private Vector3 lastEndPosition = Vector3.zero;
    private float curvePercentageWalked = 0f;
    private float curveLength = 0f;
    private Vector3 flockCenterPoint;

    protected override void Start()
    {
        SetNewPathPoints();
        base.Start();

        for(int i = 0; i < numberOfBats; i++)
        {
            SpawnElite();
        }
    }

    public override void UpdateActiveEnemies()
    {        
        float distanceToPlayer = (this.transform.position - Player.activePlayer.transform.position).x;
        float currentCatchUpMultiplier = distanceToPlayer > 0 ? 1 : Mathf.Lerp(1f, catchUpMultiplier, Mathf.Abs(distanceToPlayer / catchUpMaxDistance));
        float speed = flockSpeed * currentCatchUpMultiplier;

        float dt = speed / curveLength;
        curvePercentageWalked += dt*Time.deltaTime;
        curvePercentageWalked = Mathf.Min(curvePercentageWalked, 1f);

        flockCenterPoint = pathCurve.GetPointAt(curvePercentageWalked);

        if (curvePercentageWalked > 0.99f)
        {
            SetNewPathPoints();
        }

        foreach(EliteEnemy elite in activeEnemies)
        {
            (elite as Bat).pathPoint = flockCenterPoint;
        }

        base.UpdateActiveEnemies();
    }

    private void SetNewPathPoints()
    {
        float startPosition = flockCenterPoint.x;
        float endPosition = startPosition + minPathLength + randomPathBonusLength * Random.Range(0f, 1f);
        if(pathCurve[0].position.x > 0f)
        {
            pathCurve[0].position = pathCurve[1].position;
        }
        else
        {
            pathCurve[0].position = World.activeWorld.RandomPathTileAtXPosition((int)startPosition).transform.position;
        }
        pathCurve[1].position = World.activeWorld.RandomPathTileAtXPosition((int)endPosition).transform.position;
        curveLength = pathCurve.length;
        curvePercentageWalked = 0f;
    }
}
