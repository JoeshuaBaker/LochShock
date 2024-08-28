using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttacksAudio : MonoBehaviour
{
    //AXE SECTION
    public void PlayAxe()
    {
        AkSoundEngine.PostEvent("PlayAxe", gameObject);
        return;
    }

    //MACE SECTION
    public void PlayMace()
    {
        AkSoundEngine.PostEvent("PlayMace", gameObject);
        return;
    }

    //DAGGER SECTION
    public void PlayDagger()
    {
        AkSoundEngine.PostEvent("PlayDagger", gameObject);
        return;
    }

    //SPEAR SECTION
    public void PlaySpear()
    {
        AkSoundEngine.PostEvent("PlaySpear", gameObject);
        return;
    }

    //SWORD SECTION
    public void PlaySword()
    {
        AkSoundEngine.PostEvent("PlaySword", gameObject);
        return;
    }

}
