using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MonoBehaviour
{

    public Animator settingsAnimator;


    public void OnSettingsButtonPressed()
    {
        settingsAnimator.Play("SettingsMenuIntro");
    }

    public void OnSettingsReturnButtonPressed()
    {
        settingsAnimator.Play("SettingsMenuOutro");
    }

}
