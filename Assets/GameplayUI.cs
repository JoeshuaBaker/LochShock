using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text distanceText;
    public TMP_Text distanceTextBoss;
    public TMP_Text moneyText;
    public TMP_Text orbText;
    public TMP_Text orbTextBomb;
    public TMP_Text bombText;
    public Image bombFrame;
    public Color frameColor;
    public Color frameColorRed;
    public Transform healthParent;
    public List<Animator> healthList;
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
    public TMP_Text bossName;

    public float grappleCD;
    public float grappleCDMax;

    public Animator signalLost;
    public TMP_Text endText;

    public ItemMicroFrame[] microFrames;
    public Item[] items;
    public Gun activeGun;

    //Audio Variables
    private bool missionEndSound = true;
   
    //Animator State Names For Health
    private string fullHealthAnim = "UIHeartBeat";
    private string emptyHealthAnim = "UIHeartEmpty";
    private string fullArmorAnim = "UIArmorShine";
    private string emptyArmorAnim = "UIArmorEmpty";

    private void Start()
    {
        SetHp(Player.activePlayer.currentHp, Player.activePlayer.maxHp);
        SetOrbs(0);
        missionEndSound = true;
    }

    private void Update()
    {
        SetTime();
        SetDistance();

        if(items != null && items.Length > 0)
        {
            for (int i = 0; i < microFrames.Length - 1; i++)
            {
                
                microFrames[i + 1].item = items[i];

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

    public void SetOrbs(int orbs , int orbsToBomb = 0)
    {
        orbText.text = $"x{orbs}";

        orbTextBomb.text = $"x{orbsToBomb})";

        if (orbs >= orbsToBomb)
        {
            bombFrame.color = frameColor;
            bombText.text = "BOMB ENABLED";
        }
        else
        {
            bombFrame.color = frameColorRed;
            bombText.text = "NOT ENOUGH ORBS";
        }
    }

    public void SetMoney(int money)
    {
        moneyText.text = $"x{money}";
    }

    public void SetTime()
    {
        if(!Player.activePlayer.isDead && !bossDead)
        {
            timeText.text = $"<mspace=25>{((int)(Time.timeSinceLevelLoad / 60f)).ToString("00")}</mspace>:<mspace=25>{((int)(Time.timeSinceLevelLoad % 60f)).ToString("00")}";
        }
    }

    public void SetDistance()
    {
        distanceText.text = $"<mspace=25>{(int)Mathf.Max(0f, Player.activePlayer.transform.position.x)}</mspace>m";
    }

    public void SetBossDistance(float bossDistance)
    {
        distanceTextBoss.text = $"<mspace=25> {(int)bossDistance}</mspace>m";
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
            //Audio Section
            if (missionEndSound)
            {
                AkSoundEngine.PostEvent("PlayBossSeedMissionClear", this.gameObject);
                missionEndSound = false;
            }
        }
    }

    public void RestartGame()
    {
        Debug.Log("Game Restarted");
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void TrackBoss()
    {

    }

    public void StopTrackBoss()
    {

    }

    public void SetupBossBar(string bossNameAndTitle)
    {
        bossName.text = bossNameAndTitle;
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
