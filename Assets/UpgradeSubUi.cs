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
    public TMP_Text skipScrap;
    public int currentValue;

    public bool isSetup;
    public bool animate;
    public bool checkOutro;
    public bool skipping;
    public bool taking;

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

    public void SetUpgradeItems(Item[] items, int upgradeValue = 0)
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
        }

        //scrap pick options
        currentValue = upgradeValue;

        for (int i = 0; i < itemFrames.Length; i++)
        {
            itemAdditionalScrap[i].text = $"+{currentValue - items[i].disassembleValue}";
        }

        skipScrap.text = $"+{currentValue}";

        skipping = false;
        taking = false;

        FocusUpgradeUi();
    }

    public void DismissUpgradeUi(bool closeMenus = false)
    {
        animator.Play("UpgradeSubUiOutro");

        for(int i = 0; i < itemFrames.Length; i++)
        {
            itemFrames[i].PlayCardOutro(-.2f);
        }

        if (closeMenus)
        {
            invUpgradeUi.UiClose(true);
        }

        checkOutro = true;
    }

    public void FocusUpgradeUi()
    {
        this.gameObject.SetActive(true);

        animator.Play("UpgradeSubUiIntro");

        animate = true;

        for (int i = 0; i < itemFrames.Length; i++)
        {
            itemFrames[i].PlayCardIntro(-.2f, true);
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
        if (taking)
        {
            return;
        }

        if (inventory.HasNonStashSpaceFor(frame.item))
        {
            inventory.AddItem(frame.item);

            frame.PlayUpgradeEffect();

            if (currentValue != 0)
            {
                int gainScrap = currentValue - frame.item.disassembleValue;
                inventory.AddScrap(gainScrap);
                invUpgradeUi.UpdateScrapAmount();
            }

            //Audio Section
            AkSoundEngine.PostEvent("PlayButtonPress", this.gameObject);

            taking = true;

            DismissUpgradeUi(true);
        }
        else
        {
            bool addedItem = inventory.AddItem(frame.item);
            if (addedItem)
            {

                frame.PlayUpgradeEffect();
                frame.PlayContextMessage("STASHED");
                
                if(currentValue != 0)
                {
                    int gainScrap = currentValue - frame.item.disassembleValue;
                    inventory.AddScrap(gainScrap);
                    invUpgradeUi.UpdateScrapAmount();
                }

                //Audio Section
                AkSoundEngine.PostEvent("PlayButtonPress", this.gameObject);

                taking = true;

                DismissUpgradeUi(true);
            }
            else
            {
                frame.PlayContextMessage("FULL");
                frame.PlayCardShake();
            }
        }
    }

    public void OnSkipButtonPressed()
    {
        if (skipping)
        {
            return;
        }

        if(currentValue != 0)
        {
            int gainScrap = currentValue;
            inventory.AddScrap(gainScrap);
            invUpgradeUi.UpdateScrapAmount();
        }

        skipping = true;

        DismissUpgradeUi(true);
    }
}
