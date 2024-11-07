using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsMenu : MonoBehaviour
{

    public Animator creditsAnimator;

    public void OnCreditsButtonPressed()
    {
        creditsAnimator.Play("CreditsMenuIntro");
    }

    public void OnCreditsReturnButtonPressed()
    {
        creditsAnimator.Play("CreditsMenuOutro");
    }

}
