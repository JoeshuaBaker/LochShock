using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeAttack : MonoBehaviour
{
    public Animator weaponAnimator;
    public string weaponAnimationName;

    public SpriteRenderer axe;
    public SpriteRenderer axeEmitter;
    public ParticleSystem spearParticleSystem;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.V))
        {
            axe.enabled = true;
            axeEmitter.enabled = true;
            spearParticleSystem.Play();

            weaponAnimator.Play(weaponAnimationName);
        }

        else
        {
            axe.enabled = false;
            axeEmitter.enabled = false;
            
            spearParticleSystem.Pause();

            weaponAnimator.Play("Blend Tree");

            spearParticleSystem.Clear();
        }

    }
}
