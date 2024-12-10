using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatMissile : MonoBehaviour
{
    public AnimationCurve simulatedHeightCurve;
    public float simulatedHeightUnits;
    public float heightScaleFactor;
    public float heightOpacityFactor;
    public float spinsPerSecond = 2f;
    public SpriteRenderer sprite;

    private float travelTime;
    private float totalTime;
    private float eulerX;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 midPoint;
    private Vector3 tangent;
    private Vector3 lastPoint;
    public void Fire(float travelTime, Vector3 startPoint, Vector3 endPoint)
    {
        this.gameObject.SetActive(true);
        this.transform.position = startPoint;
        this.travelTime = 0;
        totalTime = travelTime;
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        lastPoint = startPoint;
        tangent = new Vector2((endPoint.y - startPoint.y), (endPoint.x - startPoint.x) * -1).normalized;
        midPoint = new Vector3((startPoint.x + endPoint.x) / 2f, (startPoint.y + endPoint.y) / 2f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(travelTime < totalTime)
        {
            float progress = travelTime / totalTime;
            float curveValue = simulatedHeightCurve.Evaluate(progress);

            this.transform.position = Vector3.Lerp(startPoint, endPoint, progress) + Vector3.up * curveValue * simulatedHeightUnits;
            this.transform.right = lastPoint - this.transform.position;
            lastPoint = this.transform.position;

            //this.transform.rotation = Quaternion.Euler(eulerX, 0f, this.transform.rotation.eulerAngles.z);

            this.transform.localScale = Vector3.one + Vector3.one * curveValue * heightScaleFactor;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f - (curveValue*curveValue));
            travelTime += Time.deltaTime;
            if (travelTime >= totalTime)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
