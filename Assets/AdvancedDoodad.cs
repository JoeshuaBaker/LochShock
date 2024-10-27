using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedDoodad : MonoBehaviour
{
    public Identity currentIdentity;

    public enum Identity
    {
        tree,
        spire,
        debris,
        off,
        onPath,
        onPathBasic,
        onPathRare
    }

    public float animationDuration;
    public float currentAnimTime;
    public Sprite[] assignedSprites;

    [System.Serializable]
    public class OffpathSprites
    {
        public Sprite[] sprites;
    }

    public List<OffpathSprites> possibleSpritesOffPath;
    public Sprite[] possibleSpritesOnPath;
    public Sprite[] possibleSpritesOnPathBasic;
    public Sprite[] possibleSpritesOnPathRare;
    public float timePerFrame;
    public SpriteRenderer currentSprite;
    public bool animatedDoodad = true;

    public GameObject bonusDoodad;
    public SpriteRenderer bonusDoodadSprite;
    public Sprite[] bonusDoodadPossibleSprites;
    public bool usesBonusDoodad = true;

    public bool onPath;
    public bool oversizedEnabled;

    public Vector3 swayValue;
    public float swayStrength;
    public float swayStrengthFloor;
    public float swaySpeed;
    public float swaySpeedFloor;
    public float stiffness;
    public float stiffnessMin;
    public float stiffnessMax;
    public float swayOffset;
    public float bonusSwaySpeedDecay;
    public float bonusSwayStrengthDecay;

    public GameObject colliderParent;
    public BoxCollider2D boxCollider;
    public float closestTile;
    public float distanceToTreeline = 15f;
    public float minRangeOffPath = 1.42f;
    public float doodadGradientProbability;
    public float offpathDoodadProbability;
    public float onpathDoodadProbability;
    public float spireAmount;
    public float debrisAmount;

    public bool destroyed;
    
    void Start()
    {
        swayStrengthFloor = swayStrength;
        swaySpeedFloor = swaySpeed;

        stiffness = Random.Range(stiffnessMin, stiffnessMax);

        swayOffset = Random.Range(-1f, 1f);
    }

    void Update()
    {
        currentAnimTime += Time.deltaTime;

        if (animatedDoodad)
        {
            AnimateDoodad();
        }
        if (usesBonusDoodad)
        {
            Sway();
        }
      
    }

    public void AnimateDoodad()
    {
        if(assignedSprites == null || assignedSprites.Length == 0)
        {
            return;
        }

        currentAnimTime %= animationDuration;

        timePerFrame = animationDuration / assignedSprites.Length;

        currentSprite.sprite = assignedSprites[(int)(currentAnimTime / timePerFrame)];
    }

    public void Sway()
    {
        swayValue = new Vector3(0f, 0f, Mathf.Sin((Time.time + swayOffset) * (swaySpeed * stiffness)) * (swayStrength * stiffness));

        bonusDoodad.transform.eulerAngles = swayValue;

        swaySpeed = Mathf.Max(swaySpeedFloor, swaySpeed * (bonusSwaySpeedDecay * Time.deltaTime));
        swayStrength = Mathf.Max(swayStrengthFloor, swayStrength * (bonusSwayStrengthDecay * Time.deltaTime));
    }

    public void UpdateIdentity()
    {
        colliderParent.SetActive(false);
        this.gameObject.SetActive(true);
        usesBonusDoodad = false;
        bonusDoodadSprite.enabled = false;

        Color spColor = currentSprite.color;
        spColor.a = 1f;
        currentSprite.color = spColor;

        closestTile = World.activeWorld.ClosestTileDistance(this.transform.position);

        //choosing identity
        if (closestTile < minRangeOffPath)
        {
            animatedDoodad = false;

            Bounds bounds = boxCollider.bounds;
            bounds.center = this.transform.position;

            onPath = World.activeWorld.IsBoundsOnPath(bounds);

            if (onPath)
            {
                float random = Random.Range(0f, 1f);

                if(random < 0.5f)
                {
                    currentIdentity = Identity.onPathBasic;
                }
                else
                {
                    if (random < 0.99f)
                    {
                        currentIdentity = Identity.onPath;
                    }
                    else
                    {
                        currentIdentity = Identity.onPathRare;
                    }
                }
            }
            else
            {
                currentIdentity = Identity.onPathBasic;
            }
        }
        else 
        {
            animatedDoodad = true;

            float randomNumber = Random.Range(0f, 1f);

            if(randomNumber <= spireAmount)
            {
                currentIdentity = Identity.spire;
            }
            else if (randomNumber <= debrisAmount + spireAmount)
            {
                currentIdentity = Identity.debris;
            }
            else
            {
                float gradient = closestTile / (distanceToTreeline + minRangeOffPath);
                float grade = Random.Range(0f, 1f);
                float distribution = Random.Range(0f, 1f);

                if (gradient < grade)
                {
                    currentIdentity = Identity.debris;
                }
                else
                {
                    currentIdentity = Identity.tree;
                }

                if (distribution > (gradient * (1f - doodadGradientProbability) + doodadGradientProbability))
                {
                    currentIdentity = Identity.off;
                }
            }  

            if(randomNumber > offpathDoodadProbability)
            {
                currentIdentity = Identity.off;
            }

        }

        //setting values based on identity

        if (currentIdentity == Identity.spire)
        {
            assignedSprites = possibleSpritesOffPath[2].sprites;
        }
        else if (currentIdentity == Identity.debris)
        {
            assignedSprites = possibleSpritesOffPath[Random.Range(0, possibleSpritesOffPath.Count)].sprites;
        }
        else if (currentIdentity == Identity.tree)
        {
            colliderParent.SetActive(true);

            usesBonusDoodad = true;
            bonusDoodadSprite.enabled = true;
            int randomSprite = Random.Range(0, bonusDoodadPossibleSprites.Length);
            bonusDoodadSprite.sprite = bonusDoodadPossibleSprites[randomSprite];

            assignedSprites = possibleSpritesOffPath[0].sprites;
        }
        else if (currentIdentity == Identity.off)
        {
            this.gameObject.SetActive(false);
        }
        else if (currentIdentity == Identity.onPathBasic)
        {
            if (possibleSpritesOnPathBasic == null || possibleSpritesOnPathBasic.Length == 0)
            {
                return;
            }

            currentSprite.sprite = possibleSpritesOnPathBasic[Random.Range(0, possibleSpritesOnPathBasic.Length)];
            Color spriteColor = currentSprite.color;
            spriteColor.a = 0.6f;
            currentSprite.color = spriteColor;
        }
        else if (currentIdentity == Identity.onPath)
        {
            if (possibleSpritesOnPath == null || possibleSpritesOnPath.Length == 0)
            {
                return;
            }

            currentSprite.sprite = possibleSpritesOnPath[Random.Range(0, possibleSpritesOnPath.Length)];
        }
        else if (currentIdentity == Identity.onPathRare)
        {
            if (possibleSpritesOnPathRare == null || possibleSpritesOnPathRare.Length == 0)
            {
                return;
            }

            currentSprite.sprite = possibleSpritesOnPathRare[Random.Range(0, possibleSpritesOnPathRare.Length)];
        }
    }

    public void AddSway(float swaySpeedAdded, float swayStrengthAdded)
    {
        swaySpeed += swaySpeedAdded;
        swayStrength += swayStrengthAdded;
    }

    public void Destruct()
    {
        if (!destroyed)
        {
            bonusDoodad.SetActive(false);
            assignedSprites = possibleSpritesOffPath[Random.Range(0, 2)].sprites;
            colliderParent.SetActive(false);

        }

    }
}
