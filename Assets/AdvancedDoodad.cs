using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedDoodad : MonoBehaviour
{
    public float animationDuration;
    public float currentAnimTime;
    public Sprite[] assignedSprites;
    public List<Sprite[]> possibleSpritesOffPath;
    public List<Sprite[]> possibleSpritesOnPath;
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

        closestTile = World.activeWorld.ClosestTileDistance(this.transform.position);

        if(closestTile < 1.42f)
        {
            currentSprite.enabled = false;
            bonusDoodadSprite.enabled = false;
            //Bounds bounds = boxCollider.bounds;
            //bounds.center = this.transform.position;
            //onPath = World.activeWorld.IsBoundsOnPath(boxCollider.bounds);
            //if (onPath)
            //{
            //    this.gameObject.SetActive(true);
            //}

            //animatedDoodad = false;

        }
        else if (closestTile < 2.8f)
        {
            usesBonusDoodad = false;
            currentSprite.enabled = true;
            bonusDoodadSprite.enabled = false;

            animatedDoodad = true;
            this.gameObject.SetActive(true);
        }
        else
        {
            usesBonusDoodad = true;
            currentSprite.enabled = true;
            bonusDoodadSprite.enabled = true;

            animatedDoodad = true;
            this.gameObject.SetActive(true);

            int randomSprite = Random.Range(0, bonusDoodadPossibleSprites.Length);
            bonusDoodadSprite.sprite = bonusDoodadPossibleSprites[randomSprite];
        }
        

        //check where you are

        // if off path > check off path distance
        //offpathdoodads are animated doodads

        // decide what to be based on distance
        // larger doodads useBonusDoodad

        // on path check larger area
        //allow oversizedEnabled

        // decide what to be based on area
    }

    public void Destruct()
    {
        bonusDoodad.SetActive(false);

        //some particle system function call, like bullet particle hit

    }
}
