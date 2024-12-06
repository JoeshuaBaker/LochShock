using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDataFrame
{
    void PlayContextMessage(String message, float duration = 1f);


    void PlayCardShake();


    void PlayCardIntro(float maxDelay = 0f, bool playShineEffect = false);


    void PlayCardOutro(float maxDelay = 0f);


    void PlayUpgradeEffect();


    void PlayRecycleEffect();


    void OnBaseButtonPressed();


    void OnSelect();


    void OnPointerEnter();


    void OnPointerExit();


    void OnDeselect();


    void UpdateTabText();


    void SetItem(Item item);


    Item GetItem();

}
