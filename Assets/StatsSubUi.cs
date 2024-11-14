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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
