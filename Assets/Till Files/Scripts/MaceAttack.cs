using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceAttack : MonoBehaviour
{
    public Animator weaponAnimator;
    public string weaponAnimationName;

    //public SpriteRenderer mace;
    //public SpriteRenderer maceEmitter;
    public ParticleSystem spearParticleSystem;
    public ParticleSystem streakParticleSystem;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.V))
        {
            //axe.enabled = true;
            //axeEmitter.enabled = true;
            spearParticleSystem.Play();
            streakParticleSystem.Play();

            weaponAnimator.Play(weaponAnimationName);
        }

        else
        {
            //axe.enabled = false;
            //axeEmitter.enabled = false;

            spearParticleSystem.Pause();
            streakParticleSystem.Pause();

            weaponAnimator.Play("Blend Tree");

            //spearParticleSystem.Clear();
        }

    }
}
