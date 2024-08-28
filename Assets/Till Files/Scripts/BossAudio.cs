using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudio : MonoBehaviour
{

    public void SeedStopAllAudio()
    {
        AkSoundEngine.PostEvent("PlaySeedStopAllAudio", this.gameObject);
        return;
    }

    public void SeedWhooshCut()
    {
        AkSoundEngine.PostEvent("PlayBossSeedWhooshCut", this.gameObject);
        return;
    }

    public void SeedSlam()
    {
        AkSoundEngine.PostEvent("PlayBossSeedSlam", this.gameObject);
        return;
    }

    public void SeedSpray()
    {
        AkSoundEngine.PostEvent("PlayBossSeedSpray", this.gameObject);
        return;
    }

    public void SeedExplode()
    {
        AkSoundEngine.PostEvent("PlayBossSeedExplode", this.gameObject);
        return;
    }

    public void SeedMissionClear()
    {
        AkSoundEngine.PostEvent("PlayBossSeedMissionClear", this.gameObject);
        return;
    }

}
