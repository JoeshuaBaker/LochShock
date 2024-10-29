using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAudioEvent : MonoBehaviour
{
    public void PlayPathArrow()
    {
        AkSoundEngine.PostEvent("PlayPathArrow", this.gameObject);
        return;
    }
}
