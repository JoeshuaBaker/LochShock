using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{

    public Animator weaponAnimator;
    public string weaponAnimationName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.V))
        {
            weaponAnimator.Play(weaponAnimationName);
        }

        else
        {
            weaponAnimator.Play("Blend Tree");
        }

    }
}
