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
    public Transform healthParent;
    public List<Animator> healthList;
    public Transform orbParent;
    public List<Image> orbList;
    public Animator healthPrefab;
    public Image orbPrefab;
    public GameObject bombReminder;

    public float grappleCD;
    public float grappleCDMax;

    public Animator signalLost;

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
    

        if(Input.GetKeyDown(KeyCode.Q) && bombReminder != null)
        {
            bombReminder.gameObject.SetActive(false);
        }

        grappleCD = Player.activePlayer.grappleCoolDownCurrent;
        grappleCDMax = Player.activePlayer.grapplingCoolDownBase;

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

    public void SetOrbs(int orbs)
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
            orbList[i].gameObject.SetActive(i < orbs);
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
        if(!Player.activePlayer.isDead)
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
        signalLost.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
