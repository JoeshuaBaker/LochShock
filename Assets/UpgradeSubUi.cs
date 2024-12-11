using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeSubUi : MonoBehaviour
{

    public Animator animator;
    public InventoryUpgradeUi invUpgradeUi;
    public Inventory inventory;
    public GameObject[] panelComponents;

    public GameObject[] wingRoots;
    public GameObject[] feathersLeft;
    public GameObject[] feathersRight;

    public GameObject[] eyes;
    public GameObject[] flaps;
    public GameObject[] bars;

    public GameObject[] eyesRot;
    public GameObject[] finsClose;
    public GameObject[] finsFar;

    public GameObject[] orbs;

    public float panelBob = 8f;
    public float panelBobOffset = -.3f;

    public float wingRot = 3f;
    public float wingRotOffset = 1f;

    public float eyeRot;

    public GameObject[] iris;
    public GameObject[] pupil;

    public bool[] moveEye;
    public Vector3[] eyeStartPos;
    public Vector3[] eyeTargetPos;
    public float[] eyeMoveTimeCurrent;
    public float[] eyeMoveTransition;
    public float[] eyeMoveTime;
    public float eyeMoveRange = 12f;
    public float minTimeToMove = 0.5f;
    public float maxTimeToMove = 5f;
    public float eyeTransitionTime = 0.5f;

    public ItemDataFrame[] itemFrames;
    public TMP_Text[] itemAdditionalScrap;
    public GameObject[] itemFrameParents;
    public TMP_Text skipScrap;
    public int currentValue;

    public bool isSetup;
    public bool animate;
    public bool checkOutro;
    public bool newShop = true;

    public int rerollCost = 100;
    public int rerollCostBase = 100;
    public int rerollCostGrowth = 50;

    public Item[] showItems;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animate)
        {
            AnimateDetails();
        }

        if (checkOutro)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

            if (animState.IsName("UpgradeSubUiOutro") && animState.normalizedTime >= 1f)
            {
                this.gameObject.SetActive(false);
                checkOutro = false;
                animate = false;
            }
        }
    }

    public void SetUp()
    {
        inventory = Player.activePlayer.inventory;
    }

    public void SetUpgradeItems(Item[] items, int upgradeValue = 0, bool reroll = false)
    {
        if (!isSetup)
        {
            SetUp();
        }

        //Audio Section
        AkSoundEngine.PostEvent("PlayOrbGet", this.gameObject);

        for (int i = 0; i < itemFrames.Length; i++)
        {
            Item item = i < items.Length ? items[i] : null;

            itemFrames[i].SetItem(item);

            if(item != null)
            {
                itemFrameParents[i].SetActive(true);
                //itemAdditionalScrap[i].text = $"-{items[i].disassembleValue}";
            }
            else
            {
                itemFrameParents[i].SetActive(false);
            }
        }

        skipScrap.text = $"REROLL (<mspace=15>{rerollCost}</mspace>)";

        if (!reroll)
        {
            FocusUpgradeUi();
        }
        else
        {

            ResetPurchasedCards();
            IntroduceCards(); 

        }
    }

    public void ResetPurchasedCards()
    {
        for (int i = 0; i < itemFrames.Length; i++)
        {
            itemFrames[i].purchased = false;
        }
    }

    public void DismissUpgradeUi(bool resetShop = false)
    {
        animator.Play("UpgradeSubUiOutro");

        for(int i = 0; i < itemFrames.Length; i++)
        {
            itemFrames[i].PlayCardOutro(-.2f);
        }

        if (resetShop)
        {
            newShop = true;
        }

        checkOutro = true;
    }

    public void FocusUpgradeUi()
    {
        this.gameObject.SetActive(true);

        animator.Play("UpgradeSubUiIntro");

        animate = true;

        if (newShop)
        {
            ResetPurchasedCards();
            newShop = false;
        }

        IntroduceCards();
    }

    public void IntroduceCards()
    {
        for (int i = 0; i < itemFrames.Length; i++)
        {
            itemFrames[i].PlayCardIntro(-.2f, true);
            RefreshTabs();
        }
    }

    public void RefreshTabs()
    {
        for (int i = 0; i < itemFrames.Length; i++)
        {
            itemFrames[i].UpdateTabText();
        }
    }
    public void AnimateDetails()
    {

        for(int i = 0; i < panelComponents.Length; i++)
        {
            panelComponents[i].transform.localPosition = new Vector3(0f,(int)(Mathf.Sin(Time.unscaledTime + (panelBobOffset * i)) * panelBob), 0f);
        }

        var quat = new Quaternion(0f, 0f, 0f, 0f);

        Vector3 rot = new Vector3(0f, 0f, Mathf.Sin(Time.unscaledTime + wingRotOffset) * wingRot);

        quat.eulerAngles = rot;

        for(int i = 0; i < feathersLeft.Length; i++)
        {
            feathersLeft[i].transform.localRotation = quat;
            feathersRight[i].transform.localRotation = quat;
        }

        for(int i = 0; i < eyes.Length; i++)
        {
            eyes[i].transform.localPosition = new Vector3(0f, Mathf.Sin(Time.unscaledTime + ( -0.8f)) * panelBob * 1.5f, 0f);
            if(i < 2)
            {
                flaps[i].transform.localPosition = new Vector3(0f, Mathf.Sin(Time.unscaledTime + (-0.85f)) * panelBob, 0f);
                bars[i].transform.localPosition = new Vector3(0f, Mathf.Sin(Time.unscaledTime + (-.9f)) * panelBob, 0f);
            }
            else
            {
                flaps[i].transform.localPosition = new Vector3(0f, Mathf.Sin(Time.unscaledTime + (-0.7f)) * -panelBob, 0f);
                bars[i].transform.localPosition = new Vector3(0f, Mathf.Sin(Time.unscaledTime + (-.6f)) * -panelBob, 0f);
            }
          }

        rot = new Vector3(0f, 0f, Mathf.Sin(Time.unscaledTime - 0.1f) * eyeRot);

        quat.eulerAngles = rot;

        for (int i = 0; i< eyesRot.Length; i++)
        {
            eyesRot[i].transform.localRotation = quat;
        }

        rot = new Vector3(0f, 0f, Mathf.Sin(Time.unscaledTime - 0.15f) * eyeRot);

        quat.eulerAngles = rot;

        for(int i = 0; i < finsClose.Length; i++)
        {
            finsClose[i].transform.localRotation = quat;
        }

        rot = new Vector3(0f, 0f, Mathf.Sin(Time.unscaledTime - 0.3f) * eyeRot);

        quat.eulerAngles = rot;

        for (int i = 0; i < finsFar.Length; i++)
        {
            finsFar[i].transform.localRotation = quat;
        }


        for(int i = 0; i< moveEye.Length; i++)
        {
            if (moveEye[i])
            {
                iris[i].transform.localPosition = new Vector2(Mathf.Lerp(eyeStartPos[i].x, eyeTargetPos[i].x, eyeMoveTransition[i]), Mathf.Lerp(eyeStartPos[i].y, eyeTargetPos[i].y, eyeMoveTransition[i]));
                pupil[i].transform.localPosition = new Vector2(Mathf.Lerp(eyeStartPos[i].x, eyeTargetPos[i].x, eyeMoveTransition[i]), Mathf.Lerp(eyeStartPos[i].y, eyeTargetPos[i].y, eyeMoveTransition[i]));

                eyeMoveTransition[i] += (Time.unscaledDeltaTime / eyeTransitionTime);

                if (eyeMoveTransition[i] > 1f)
                {
                    moveEye[i] = false;

                    eyeMoveTime[i] = Random.Range(minTimeToMove, maxTimeToMove);
                }

            }
            else
            {

                eyeMoveTimeCurrent[i] += Time.unscaledDeltaTime;
                if (eyeMoveTimeCurrent[i] > eyeMoveTime[i])
                {
                    moveEye[i] = true;

                    eyeMoveTimeCurrent[i] = 0f;

                    eyeStartPos[i] = iris[i].transform.localPosition;

                    eyeTargetPos[i] = new Vector2(Random.Range(-eyeMoveRange, eyeMoveRange), Random.Range(-eyeMoveRange, eyeMoveRange));

                    eyeMoveTransition[i] = 0f;
                }
            }
        }
    }

    public void Take(ItemDataFrame frame)
    {

        if (frame.purchased)
        {
            return;
        }

        if(inventory.scrap < frame.item.disassembleValue)
        {
            frame.PlayContextMessage("LOL POOR");
            frame.PlayCardShake();

            return;
        }

        if (inventory.HasNonStashSpaceFor(frame.item))
        {

            frame.PlayCardOutro(panelBobOffset, true);

            inventory.AddItem(frame.item);

            frame.PlayUpgradeEffect();

            inventory.AddScrap(-frame.item.disassembleValue);
            invUpgradeUi.UpdateScrapAmount();

            frame.purchased = true;

            //Audio Section
            AkSoundEngine.PostEvent("PlayButtonPress", this.gameObject);
        }
        else
        {
            bool addedItem = inventory.AddItem(frame.item);
            if (addedItem)
            {
                frame.PlayCardOutro(0f, true);

                frame.PlayUpgradeEffect();

                frame.PlayContextMessage("STASHED");

                inventory.AddScrap(-frame.item.disassembleValue);
                invUpgradeUi.UpdateScrapAmount();

                frame.purchased = true;

                //Audio Section
                AkSoundEngine.PostEvent("PlayButtonPress", this.gameObject);
            }
            else
            {
                frame.PlayContextMessage("FULL");
                frame.PlayCardShake();
            }
        }

        RefreshTabs();

    }

    public void OnRerollButtonPressed()
    {

        if (rerollCost <= inventory.scrap)
        {
            inventory.Orb(false, false, true, rerollCost);
            invUpgradeUi.UpdateScrapAmount();

            //Audio Section
            AkSoundEngine.PostEvent("PlayButtonPress", this.gameObject);
        }
        else
        {
            Debug.Log("poor");
        }
    }
}
