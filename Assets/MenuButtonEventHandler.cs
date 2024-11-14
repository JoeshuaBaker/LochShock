using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class MenuButtonEventHandler : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
{

    public Button button;

    public TMP_Text text;

    public TMP_FontAsset unlitFont;
    public TMP_FontAsset litFont;
    public TMP_FontAsset disabledFont;

    public void Reset()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(button.interactable == true)
        {
            button.Select();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(text != null)
        {
            text.font = litFont;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if(text != null)
        {
            text.font = unlitFont;
        }
    }

    public void DisableButton()
    {
        button.interactable = false;
        if (text != null)
        {
            text.font = disabledFont;
        }
    }

    public void EnableButton()
    {
        button.interactable = true;
        if(text != null)
        {
            text.font = unlitFont;
        }
    }

}
