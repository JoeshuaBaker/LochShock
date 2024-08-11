using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunActiveItem : ActiveItem
{
    public Gun gun;
    public bool useItemStats = true;

    private List<StatBlock> statBlocks = new List<StatBlock>();
    public override void Activate()
    {
        if(cooldownTimer <= 0f)
        {
            Debug.Log("Inside activate.");
            Vector2 mouseDirection = Player.activePlayer.mouseDirection;
            gun.shooting = true;
            gun.transform.localEulerAngles = Quaternion.FromToRotation(Vector3.right, new Vector3(mouseDirection.x, mouseDirection.y, 0f)).eulerAngles;

            statBlocks.Clear();
            if (useItemStats)
            {
                foreach (Item item in Player.activePlayer.inventory.items)
                {
                    if (item != null)
                    {
                        statBlocks.AddRange(item.newStatsList);
                    }
                }
            }

            statBlocks.AddRange(gun.newStatsList);
            this.combinedStats.UpdateSources(statBlocks);
            gun.ApplyNewStatBlock(combinedStats);
            gun.emitter.ApplyStatBlock(combinedStats);
            gun.emitter.Direction = mouseDirection;
            gun.magazine = gun.maxMagazine;
            gun.Shoot();
            cooldownTimer = cooldown;
            gun.magazine = gun.maxMagazine;
            gun.shooting = false;
        }
    }

    public override void Setup()
    {
        gun = Instantiate(gun, this.transform);
        gun.name = gun.name.Replace("(Clone)", "");
        statBlocks = new List<StatBlock>();
    }

    public override void LevelUp()
    {
        gun.LevelUp();
        level++;
        cooldown = cooldown * 0.95f;
    }

    public override StatBlockContext GetStatBlockContext()
    {
        StatBlockContext statBlockContext = new StatBlockContext();
        statBlockContext.AddGenericTooltip($"Fires a {StatBlockContext.GoodColor}{gun.name}</color>. Cooldown: {StatBlockContext.HighlightColor}{cooldown}</color>s.");
        return statBlockContext;
    }
}
