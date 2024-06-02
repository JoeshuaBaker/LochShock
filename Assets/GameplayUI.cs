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
    public List<Image> healthList;
    public Transform orbParent;
    public List<Image> orbList;
    public Image healthPrefab;
    public Image orbPrefab;

    public Animator signalLost;

    //Health Images
    public Sprite emptyHealthSprite;
    public Sprite fullHealthSprite;
    public Sprite emptyArmorSprite;
    public Sprite fullArmorSprite;

    private void Start()
    {
        SetHp(Player.activePlayer.currentHp, Player.activePlayer.maxHp);
        SetOrbs(0);
    }

    private void Update()
    {
        SetTime();
        SetDistance();
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

        for(int i = 0; i < healthList.Count; i++)
        {
            healthList[i].gameObject.SetActive(true);

            if (i < currentHp)
            {
                healthList[i].sprite = fullHealthSprite;
            }
            else if (i < maxHp)
            {
                healthList[i].sprite = emptyHealthSprite;
            }
            else if (currentArmor > -1 && i - maxHp < currentArmor)
            {
                healthList[i].sprite = fullArmorSprite;
            }
            else if (maxArmor > -1 && i - maxHp < maxArmor)
            {
                healthList[i].sprite = emptyArmorSprite;
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
        timeText.text = $"{(int)(Time.timeSinceLevelLoad / 60f)}:{(int)(Time.timeSinceLevelLoad % 60f)}";
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
