using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;

public class TrailTest : MonoBehaviour
{
    public static TrailTest activeTrails;
    public int bufferSize = 250;
    public TrailRenderer[] trailRenderers;
    public TrailRenderer trailPrefab;
    public int nextTrailIndex = 0;
    private void Awake()
    {
        activeTrails = this;
    }

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

    public TrailRenderer GetEnemyRenderer()
    {
        TrailRenderer renderer = GetRenderer();
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(1f, 1.0f) }
        );
        renderer.colorGradient = gradient;
        return renderer;
    }
}
