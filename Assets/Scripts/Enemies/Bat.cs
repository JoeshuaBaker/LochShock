using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : EliteEnemy
{
    public Vector3 pathPoint;
    public Vector2 pathPointOffset;
    public Vector2 offsetVelocity;
    public float maxOffsetVelocity = 4f;
    public Vector2 offsetAcceleration;
    public float randomOffsetTurnPower = 10f;
    public AnimationCurve turnPowerDistribution;
    public float swarmSize;
    private float offsetFloat;
    private float turnPower;

    public override void Setup(EliteCoordinator coordinator)
    {
        base.Setup(coordinator);

        swarmSize = (coordinator as BatCoordinator).swarmSize;
        turnPower = (coordinator as BatCoordinator).turnPower + turnPowerDistribution.Evaluate(Random.Range(0f, 1f)) * randomOffsetTurnPower;
        pathPointOffset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * swarmSize;
        maxOffsetVelocity = swarmSize;
        offsetVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * maxOffsetVelocity;
        offsetAcceleration = offsetVelocity.normalized;
        offsetFloat = Random.Range(0f, 2f*Mathf.PI);
        this.transform.position = pathPoint + pathPointOffset.xyz();
    }

    public override void CoordinatorUpdate()
    {
        offsetFloat += Time.deltaTime;
        var offsetCosin = Mathf.Cos(offsetFloat);
        var offsetSin = Mathf.Sin(offsetFloat);
        var distToPathPoint = pathPoint - this.transform.position;
        var dirToPathPoint = distToPathPoint.normalized.xy();
        offsetAcceleration = Utilities.RotateTowards(offsetAcceleration, dirToPathPoint, Mathf.Deg2Rad * distToPathPoint.magnitude * turnPower).normalized * distToPathPoint.magnitude * turnPower/swarmSize;
        offsetVelocity += (offsetAcceleration + new Vector2(offsetCosin, offsetSin)) * Time.deltaTime;
        offsetVelocity = new Vector2(Mathf.Clamp(offsetVelocity.x, -maxOffsetVelocity, maxOffsetVelocity), Mathf.Clamp(offsetVelocity.y, -maxOffsetVelocity, maxOffsetVelocity));
        pathPointOffset += offsetVelocity * Time.deltaTime;


        this.transform.position = pathPoint + pathPointOffset.xyz();
    }
}
