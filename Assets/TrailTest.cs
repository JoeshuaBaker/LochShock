using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;

public class TrailTest : MonoBehaviour
{
    public int bufferSize = 250;
    public TrailRenderer[] trailRenderers;
    public TrailRenderer trailPrefab;
    public int nextTrailIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        trailRenderers = new TrailRenderer[bufferSize];

        for(int i = 0; i < bufferSize; i++)
        {
            trailRenderers[i] = Instantiate(trailPrefab, this.transform);
            trailRenderers[i].gameObject.SetActive(false);
        }
    }

    public TrailRenderer GetRenderer()
    {
        TrailRenderer next = null;
        int i = 0;
        while ((next = trailRenderers[nextTrailIndex++]).gameObject.activeSelf && i++ < bufferSize)
        {
            nextTrailIndex %= bufferSize;
        }

        nextTrailIndex %= bufferSize;
        return next;
    }
}
