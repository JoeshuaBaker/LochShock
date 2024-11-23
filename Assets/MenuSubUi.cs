using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSubUi : MonoBehaviour
{

    public Animator animator;
    public bool menuActive;

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

            if (animState.IsName("MenuSubUiOutro") && animState.normalizedTime >= 1f)
            {
                this.gameObject.SetActive(false);
                checkOutro = false;
            }
        }
    }

    public void DismissMenuSubUi()
    {
        animator.Play("MenuSubUiOutro");
        menuActive = false;
        checkOutro = true;
    }

    public void FocusMenuSubUi()
    {
        this.gameObject.SetActive(true);
        animator.Play("MenuSubUiIntro");
        menuActive = true;
    }


    public void OnMenuButtonPressed()
    {

    }

    public void OnSettingsMenuPressed()
    {

    }

    public void OnRestartButtonPressed()
    {

    }

    public void OnReturnButtonPressed()
    {
        DismissMenuSubUi();
    }
}
