using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsSubUi : MonoBehaviour
{

    public Animator animator;

    public GameObject statTextBox;
    public GameObject CharacterTextBox;

    public TMP_Text text;

    public enum StatBeingDisplayed
    {
        pageOne,
        pageTwo,
        character
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
        if (focusInv)
        {
            animator.Play("StatsSubUiOutroInv");
        }
        else
        {
            animator.Play("StatsSubUiOutroUpgrade");
        }

        checkOutro = true;
    }

    public void FocusStats(bool focusInv)
    {
        if (focusInv)
        {
            this.gameObject.SetActive(true);
            animator.Play("StatsSubUiIntroInv");
        }
        else
        {
            this.gameObject.SetActive(true);
            animator.Play("StatsSubUiIntroUpgrade");
        }
    }

    public void OnPageOneButtonPressed()
    {
        currentDisplayedStat = StatBeingDisplayed.pageOne;
    }

    public void OnPageTwoButtonPressed()
    {
        currentDisplayedStat = StatBeingDisplayed.pageTwo;
    }

    public void OnCharacterStatsButtonPressed()
    {
        currentDisplayedStat = StatBeingDisplayed.character;
    }

    public void UpdateDisplayBasedOnState()
    {

    }

}
