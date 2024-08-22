using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text distanceText;
    public TMP_Text activeAmmoText;
    public TMP_Text inactiveAmmoText;
    public TMP_Text orbChargeText;
    public Transform healthParent;
    public List<Animator> healthList;
    public Transform orbParent;
    public List<Image> orbList;
    public Animator healthPrefab;
    public Image orbPrefab;

    public GameObject middleContainer;
    public GameObject timeContainer;
    public GameObject leftContainer;
    public GameObject bottomContainer;

    public bool bossActive;
    public Image bossHP;
    public float bossHpPercent;
    public bool bossDead;
    public bool bossDeathFinished;

    public float grappleCD;
    public float grappleCDMax;

    public Animator signalLost;
    public TMP_Text endText;

    public ItemMicroFrame[] microFrames;
    public Item[] items;
    public Gun activeGun;
   
    //Animator State Names For Health
    private string fullHealthAnim = "UIHeartBeat";
    private string emptyHealthAnim = "UIHeartEmpty";
    private string fullArmorAnim = "UIArmorShine";
    private string emptyArmorAnim = "UIArmorEmpty";

    private void Start()
    {
        SetHp(Player.activePlayer.currentHp, Player.activePlayer.maxHp);
        SetOrbs(0);
    }

    private void Update()
    {
        SetTime();
        SetDistance();

        if(items != null && items.Length > 0)
        {
            for (int i = 0; i < microFrames.Length - 1; i++)
            {
                
                if(i < 3)
                {
                    microFrames[i + 1].item = items[i];
                }
                else
                {
                    microFrames[i + 1].item = items[i+2];
                }
            }
        }

        if (bossActive)
        {
            UpdateBossBar();
        }

    }

    public void SetGrapple(float grappleCDCurrent, float grappleCDBase)
    {
        grappleCD = grappleCDCurrent;
        grappleCDMax = grappleCDBase;
    }

    private void SetAnimStateIfNotSet(Animator anim, AnimatorStateInfo info, string state, float normalizedTime)
    {
        if(!anim.gameObject.activeSelf)
        {
            anim.gameObject.SetActive(true);
        }
        if (!info.IsName(state))
        {
            anim.Play(state, 0, normalizedTime + Time.deltaTime);
        }
    }

    public void SetHp(int currentHp, int maxHp, int currentArmor = -1, int maxArmor = -1)
    {
        int iconsNeeded = Mathf.Max(maxHp + maxArmor, maxHp);
        if(iconsNeeded > healthList.Count)
        {
            for(int i = healthList.Count; i < iconsNeeded; i++)
            {
                healthList.Add(Instantiate(healthPrefab, healthParent));
            }
        }

        float syncTime = 0f;
        for(int i = 0; i < healthList.Count; i++)
        {
            Animator current = healthList[i];
            AnimatorStateInfo stateInfo = current.GetCurrentAnimatorStateInfo(0);
            if (i == 0) syncTime = stateInfo.normalizedTime;

            if (i < currentHp)
            {
                SetAnimStateIfNotSet(current, stateInfo, fullHealthAnim, syncTime);
            }
            else if (i < maxHp)
            {
                SetAnimStateIfNotSet(current, stateInfo, emptyHealthAnim, syncTime);
            }
            else if (currentArmor > -1 && i - maxHp < currentArmor)
            {
                SetAnimStateIfNotSet(current, stateInfo, fullArmorAnim, syncTime);
            }
            else if (maxArmor > -1 && i - maxHp < maxArmor)
            {
                SetAnimStateIfNotSet(current, stateInfo, emptyArmorAnim, syncTime);
            }
            else
            {
                healthList[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetOrbs(int orbs , bool orbsCharged = false, int orbsChargedNumber = 0)
    {
        if(orbs > orbList.Count)
        {
            for (int i = orbList.Count; i < orbs; i++)
            {
                orbList.Add(Instantiate(orbPrefab, orbParent));
            }
        }

        for(int i = 0; i < orbList.Count; i++)
        {
            if (!orbsCharged)
            {
                orbList[i].gameObject.SetActive(i < orbs);
            }
            else
            {
                orbList[i].gameObject.SetActive(false);
                
            }
            
        }
        if (!orbsCharged)
        {
            orbChargeText.gameObject.SetActive(false);
        }
        else
        {
            orbChargeText.text = $"NEXT ORB CHARGED x{Mathf.Min(orbsChargedNumber, 4)}";
            orbChargeText.gameObject.SetActive(true);
        }
        

    }

    public void SetAmmo(int activeCurrentAmmo, int activeMaxAmmo, int inactiveCurrentAmmo = -1, int inactiveMaxAmmo = -1)
    {
        activeAmmoText.text = $"{activeCurrentAmmo}/{activeMaxAmmo}";
        if(inactiveCurrentAmmo > -1 && inactiveMaxAmmo > -1)
        {
            inactiveAmmoText.gameObject.SetActive(true);
            inactiveAmmoText.text = $"{inactiveCurrentAmmo}/{inactiveMaxAmmo}";
        }
        else
        {
            inactiveAmmoText.gameObject.SetActive(false);
        }
    }

    public void SetTime()
    {
        if(!Player.activePlayer.isDead && !bossDead)
        {
            timeText.text = $"{((int)(Time.timeSinceLevelLoad / 60f)).ToString("00")}:{((int)(Time.timeSinceLevelLoad % 60f)).ToString("00")}";
        }
    }

    public void SetDistance()
    {
        distanceText.text = $"{(int)Mathf.Max(0f, Player.activePlayer.transform.position.x)}m";
    }

    public void ShowSignalLost()
    {
        if (!bossDeathFinished)
        {
            signalLost.gameObject.SetActive(true);
        }
        else
        {
            endText.text = "MISSION CLEAR!";
            signalLost.gameObject.SetActive(true);
        }
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateBossBar()
    {
        if (!bossDead)
        {
            bottomContainer.SetActive(true);
            bossHP.fillAmount = bossHpPercent;
        }
        if (bossDead && !bossDeathFinished)
        {
            bottomContainer.SetActive(false);
            leftContainer.SetActive(false);
            timeContainer.SetActive(false);
        }
        if (bossDeathFinished)
        {
            leftContainer.SetActive(true);
            timeContainer.SetActive(true);

            ShowSignalLost();

        }
    }
}
