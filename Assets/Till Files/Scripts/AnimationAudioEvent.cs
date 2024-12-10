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

    public void PlayBossWarningIntroG()
    {
        AkSoundEngine.PostEvent("PlayBossWarningG", this.gameObject);
        return;
    }

    public void PlayBossWarningIntroC()
    {
        AkSoundEngine.PostEvent("PlayBossWarningC", this.gameObject);
        return;
    }

    public void PlayBossSeedWarning()
    {
        AkSoundEngine.PostEvent("PlayBossSeedWarning", this.gameObject);
        return;
    }


}
