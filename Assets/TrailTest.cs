using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;

public class TrailTest : MonoBehaviour
{
    public int bufferSize = 250;
    public TrailRenderer[] trailRenderers;
    public TrailRenderer trailPrefab;
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
        foreach(var renderer in trailRenderers)
        {
            if(!renderer.gameObject.activeSelf)
            {
                //renderer.gameObject.SetActive(true);
                return renderer;
            }
        }
        return null;
    }
}
