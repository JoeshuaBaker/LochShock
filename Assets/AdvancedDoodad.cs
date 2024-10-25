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
        off
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

    public BoxCollider2D boxCollider;
    public float closestTile;
    public float doodadGradientMax = 15f;
    public float minRangeOffPath = 1.42f;
    public float doodadAppearanceBase;
    
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
        this.gameObject.SetActive(true);

        closestTile = World.activeWorld.ClosestTileDistance(this.transform.position);

        currentSprite.sortingLayerName = "Characters and Collidables";

        if(closestTile < minRangeOffPath)
        {
            bonusDoodadSprite.enabled = false;
            currentSprite.enabled = false;

            Bounds bounds = boxCollider.bounds;
            bounds.center = this.transform.position;

            onPath = World.activeWorld.IsBoundsOnPath(bounds);

            if (onPath)
            {
                if(possibleSpritesOnPath == null || possibleSpritesOnPath.Length == 0)
                {
                    return;
                }

                currentSprite.sprite = possibleSpritesOnPath[Random.Range(0, possibleSpritesOnPath.Length)];
                currentSprite.sortingLayerName = "Doodads";
                currentSprite.enabled = true;
            }
            else
            {
                this.gameObject.SetActive(false);
                currentSprite.enabled = false;
            }

            animatedDoodad = false;

        }
        else 
        {
            //choosing identity
            int randomNumber = Random.Range(1, 11);

            if(randomNumber == 1)
            {
                currentIdentity = Identity.spire;
            }
            else if (randomNumber == 2)
            {
                currentIdentity = Identity.debris;
            }
            else
            {
                float gradient = closestTile / (doodadGradientMax + minRangeOffPath);
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

                if (distribution > (gradient * (1f - doodadAppearanceBase) + doodadAppearanceBase))
                {
                    currentIdentity = Identity.off;
                }
            }

            //setting values based on identity

            if(currentIdentity == Identity.spire)
            {
                usesBonusDoodad = false;
                bonusDoodadSprite.enabled = false;

                assignedSprites = possibleSpritesOffPath[2].sprites;
            }
            else if (currentIdentity == Identity.debris)
            {
                usesBonusDoodad = false;
                bonusDoodadSprite.enabled = false;

                assignedSprites = possibleSpritesOffPath[Random.Range(0, possibleSpritesOffPath.Count)].sprites;
            }
            else if (currentIdentity == Identity.tree)
            {
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

            currentSprite.enabled = true;
            animatedDoodad = true;
           
        }
    }

    public void AddSway(float swaySpeedAdded, float swayStrengthAdded)
    {
        swaySpeed += swaySpeedAdded;
        swayStrength += swayStrengthAdded;
    }

    public void Destruct()
    {
        bonusDoodad.SetActive(false);
        assignedSprites = possibleSpritesOffPath[Random.Range(0, 2)].sprites;

        //some particle system function call, like bullet particle hit

    }
}
