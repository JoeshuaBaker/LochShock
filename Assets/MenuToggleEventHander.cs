using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuToggleEventHander : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
{
    public Toggle toggle;

    public TMP_Text text;

    public TMP_FontAsset unlitFont;
    public TMP_FontAsset litFont;

    public void Reset()
    {
        toggle = GetComponent<Toggle>();
        text = GetComponentInChildren<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        toggle.Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (text != null)
        {
            text.font = litFont;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (text != null)
        {
            text.font = unlitFont;
        }
    }
}
