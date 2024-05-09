using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleZSetter : MonoBehaviour
{

    public ParticleSystem emitter;
    ParticleSystem.Particle[] particles;

    // Start is called before the first frame update
    void Start()
    {
        particles = new ParticleSystem.Particle[emitter.main.maxParticles];
    }

    // Update is called once per frame
    void Update()
    {
        var currentAmount = emitter.GetParticles(particles);

        for ( int i =0; i < currentAmount; i++)
        {
            particles[i].position = new Vector3(particles[i].position.x, particles[i].position.y, 0f);
        }

        emitter.SetParticles(particles, currentAmount);
    }
}
