using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBolt : MonoBehaviour
{

    public ParticleSystem lightningPS;
    public ParticleSystem groundHitPS;
    public int iterations = 15;
    public float xOffset = 3f;
    public bool zeusMode;
    public CraterCreator craterCreator;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0) && zeusMode)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);

            CallLightning(worldMouse);

        }

    }

    public void CallLightning(Vector3 position)
    {
        
        this.transform.position = position;
        lightningPS.Play();

        groundHitPS.Emit(Random.Range(10, 15));

        for (int i = 0; i < iterations; i++)
        {
            Vector3 pos = this.transform.position;

            lightningPS.Emit(1);

            this.transform.position = new Vector3(Random.Range((pos.x - xOffset), (pos.x + xOffset)), Random.Range(pos.y + 1f, pos.y + 2f), 0f);

            if (i == 0)
            {
                craterCreator.CreateCrater(position, 1);
            }

        }

        


        lightningPS.Stop();
    }
}
