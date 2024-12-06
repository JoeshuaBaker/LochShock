using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDataFrameButton : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
{
    public ItemDataFrame frame;
    public StashDataFrame stashFrame;
    public IDataFrame dataFrame;

    void Start()
    {
        if(frame != null)
        {
            dataFrame = frame;
        }
        else
        {
            dataFrame = stashFrame;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
 
        dataFrame.OnDeselect();
   
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        dataFrame.OnPointerEnter();

    }

    public void OnPointerExit(PointerEventData eventData)
    {

        dataFrame.OnPointerExit();

    }

    public void OnSelect(BaseEventData eventData)
    {

        dataFrame.OnSelect();

    }

}
