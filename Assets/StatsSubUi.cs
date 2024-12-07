using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsSubUi : MonoBehaviour
{

    public Animator animator;

    public InventoryUpgradeUi invUpgUi;

    public GameObject statTextBox;
    public GameObject CharacterTextBox;

    public TMP_Text titleText;
    public TMP_Text bodyText;

    public enum StatBeingDisplayed
    {
        primaryStats,
        secondaryStats,
        characterStats,
        performanceStats
    }

    public StatBeingDisplayed currentDisplayedStat;

    public bool checkOutro;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (checkOutro)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

            if (animState.IsName("StatsSubUiOutroInv") && animState.normalizedTime >= 1f)
            {
                this.gameObject.SetActive(false);
                checkOutro = false;
            }
            if (animState.IsName("StatsSubUiOutroUpgrade") && animState.normalizedTime >= 1f)
            {
                this.gameObject.SetActive(false);
                checkOutro = false;
            }
        }
    }

    public void DismissStats(bool focusInv)
    {
        animator.Play("StatsSubUiOutroInv");
        checkOutro = true;
    }

    public void FocusStats(bool focusInv)
    {

        this.gameObject.SetActive(true);
        animator.Play("StatsSubUiIntroInv");

        UpdateStatDisplay();
    }

    public void OnPageOneButtonPressed()
    {
        currentDisplayedStat = StatBeingDisplayed.primaryStats;
        UpdateStatDisplay();
    }

    public void OnPageTwoButtonPressed()
    {
        currentDisplayedStat = StatBeingDisplayed.secondaryStats;
        UpdateStatDisplay();
    }

    public void OnCharacterStatsButtonPressed()
    {
        currentDisplayedStat = StatBeingDisplayed.characterStats;
        UpdateStatDisplay();
    }

    public void OnPerformanceButtonPressed()
    {
        currentDisplayedStat = StatBeingDisplayed.performanceStats;
        UpdateStatDisplay();
    }

    public void OnExitButtonPressed()
    {
        invUpgUi.OnStatsButtonPressed();
    }

    public void UpdateStatDisplay()
    {
       switch (currentDisplayedStat)
        {
            case StatBeingDisplayed.primaryStats:

                titleText.text = "PRIMARY STATS";

                bodyText.text = "here are your primary stats";

                break;

            case StatBeingDisplayed.secondaryStats:

                titleText.text = "SECONDARY STATS";

                bodyText.text = "and secondary stats";

                break;

            case StatBeingDisplayed.characterStats:

                titleText.text = "CHARACTER";

                bodyText.text = "character stats";

                break;

            case StatBeingDisplayed.performanceStats:

                titleText.text = "PERFORMANCE";

                bodyText.text = "damage youve done";

                break;

            default:
                break;
        }
    }

}
