using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;

[Serializable]
public class StatBlock
{
    public enum BlockType
    {
        Additive,
        Multiplicative, //Multiplicative values should be represented without the 1. E.g 20% = 0.2f, 200% = 2f
        Base,
    }

    //StatBlock properties
    public BlockType blockType;
    public PlayerStats playerStats = new PlayerStats();
    public GunStats gunStats = new GunStats();
    public Events events = new Events();
    
    [Serializable]
    public struct PlayerStats
    {
        public float health;
        public float walkSpeed;
        public float runSpeed;
        public float totalVision;
        public float visionConeAngle;
        public float visionConeRadius;
        public float visionProximityRadius;
    }

    [Serializable]
    public struct GunStats
    {
        public float magazineSize;
        public float reloadSpeed;
        public float fireSpeed;
        public float bulletStreams;
        public float bulletsPerShot;
        public float spreadAngle;
        public float accuracy;
        public float damage;
        public float speed;
        public float size;
        public float knockback;
        public float bounce;
        public float pierce;
        public float lifetime;
    }

    [Serializable]
    public class Events
    {
        public List<OnFireAction> OnFire = new List<OnFireAction>();
        public List<OnHitAction> OnHit = new List<OnHitAction>();
        public List<OnKillAction> OnKill = new List<OnKillAction>();
        public List<OnReloadAction> OnReload = new List<OnReloadAction>();
        public List<OnSecondAction> OnSecond = new List<OnSecondAction>();

        public Events Copy()
        {
            Events copy = new Events();

            copy.OnFire.AddRange(this.OnFire);
            copy.OnHit.AddRange(this.OnHit);
            copy.OnKill.AddRange(this.OnKill);
            copy.OnReload.AddRange(this.OnReload);
            copy.OnSecond.AddRange(this.OnSecond);
            return copy;
        }
    }

    public StatBlock()
    {

    }

    public StatBlock(BlockType blockType)
    {
        this.blockType = blockType;
    }

    public virtual StatBlock Copy()
    {
        StatBlock copy = new StatBlock();
        copy.events = events.Copy();
        
        return copy;
    }

    public static StatBlock Combine(IEnumerable<StatBlock> blocks)
    {
        StatBlock combinedBlock = blocks.FirstOrDefault(x => x.blockType == BlockType.Base).Copy() ?? new StatBlock(BlockType.Base);
        IEnumerable<StatBlock> additiveBlocks = blocks.Where(x => x.blockType == BlockType.Additive);

        foreach (StatBlock additiveBlock in additiveBlocks)
        {
            combinedBlock.playerStats.health                += additiveBlock.playerStats.health;
            combinedBlock.playerStats.runSpeed              += additiveBlock.playerStats.runSpeed;
            combinedBlock.playerStats.walkSpeed             += additiveBlock.playerStats.walkSpeed;
            combinedBlock.playerStats.totalVision           += additiveBlock.playerStats.totalVision;
            combinedBlock.playerStats.visionConeAngle       += additiveBlock.playerStats.visionConeAngle;
            combinedBlock.playerStats.visionConeRadius      += additiveBlock.playerStats.visionConeRadius;
            combinedBlock.playerStats.visionProximityRadius += additiveBlock.playerStats.visionProximityRadius;

            combinedBlock.gunStats.magazineSize += additiveBlock.gunStats.magazineSize;
            combinedBlock.gunStats.reloadSpeed += additiveBlock.gunStats.reloadSpeed;
            combinedBlock.gunStats.fireSpeed += additiveBlock.gunStats.fireSpeed;
            combinedBlock.gunStats.bulletStreams += additiveBlock.gunStats.bulletStreams;
            combinedBlock.gunStats.bulletsPerShot += additiveBlock.gunStats.bulletsPerShot;
            combinedBlock.gunStats.spreadAngle += additiveBlock.gunStats.spreadAngle;
            combinedBlock.gunStats.accuracy += additiveBlock.gunStats.accuracy;

            combinedBlock.gunStats.damage           += additiveBlock.gunStats.damage;
            combinedBlock.gunStats.speed            += additiveBlock.gunStats.speed;
            combinedBlock.gunStats.size             += additiveBlock.gunStats.size;
            combinedBlock.gunStats.knockback        += additiveBlock.gunStats.knockback;
            combinedBlock.gunStats.bounce           += additiveBlock.gunStats.bounce;
            combinedBlock.gunStats.pierce           += additiveBlock.gunStats.pierce;
            combinedBlock.gunStats.lifetime         += additiveBlock.gunStats.lifetime;

            combinedBlock.events.OnFire.AddRange(additiveBlock.events.OnFire);
            combinedBlock.events.OnHit.AddRange(additiveBlock.events.OnHit);
            combinedBlock.events.OnKill.AddRange(additiveBlock.events.OnKill);
            combinedBlock.events.OnReload.AddRange(additiveBlock.events.OnReload);
            combinedBlock.events.OnSecond.AddRange(additiveBlock.events.OnSecond);
        }

        //Add all multipliers together, then apply them once at the end.
        StatBlock multBlock = new StatBlock();
        IEnumerable<StatBlock> multBlocks = blocks.Where(x => x.blockType == BlockType.Multiplicative);

        foreach (StatBlock multiplicativeBlock in multBlocks)
        {
            multBlock.playerStats.health += multiplicativeBlock.playerStats.health;
            multBlock.playerStats.runSpeed += multiplicativeBlock.playerStats.runSpeed;
            multBlock.playerStats.walkSpeed += multiplicativeBlock.playerStats.walkSpeed;
            multBlock.playerStats.totalVision += multiplicativeBlock.playerStats.totalVision;
            multBlock.playerStats.visionConeAngle += multiplicativeBlock.playerStats.visionConeAngle;
            multBlock.playerStats.visionConeRadius += multiplicativeBlock.playerStats.visionConeRadius;
            multBlock.playerStats.visionProximityRadius += multiplicativeBlock.playerStats.visionProximityRadius;

            multBlock.gunStats.magazineSize += multiplicativeBlock.gunStats.magazineSize;
            multBlock.gunStats.reloadSpeed += multiplicativeBlock.gunStats.reloadSpeed;
            multBlock.gunStats.fireSpeed += multiplicativeBlock.gunStats.fireSpeed;
            multBlock.gunStats.bulletStreams += multiplicativeBlock.gunStats.bulletStreams;
            multBlock.gunStats.bulletsPerShot += multiplicativeBlock.gunStats.bulletsPerShot;
            multBlock.gunStats.spreadAngle += multiplicativeBlock.gunStats.spreadAngle;
            multBlock.gunStats.accuracy += multiplicativeBlock.gunStats.accuracy;

            multBlock.gunStats.damage       += multiplicativeBlock.gunStats.damage;
            multBlock.gunStats.speed        += multiplicativeBlock.gunStats.speed;
            multBlock.gunStats.size         += multiplicativeBlock.gunStats.size;
            multBlock.gunStats.knockback    += multiplicativeBlock.gunStats.knockback;
            multBlock.gunStats.bounce       += multiplicativeBlock.gunStats.bounce;
            multBlock.gunStats.pierce       += multiplicativeBlock.gunStats.pierce;
            multBlock.gunStats.lifetime += multiplicativeBlock.gunStats.lifetime;

            multBlock.events.OnFire.AddRange(multiplicativeBlock.events.OnFire);
            multBlock.events.OnHit.AddRange(multiplicativeBlock.events.OnHit);
            multBlock.events.OnKill.AddRange(multiplicativeBlock.events.OnKill);
            multBlock.events.OnReload.AddRange(multiplicativeBlock.events.OnReload);
            multBlock.events.OnSecond.AddRange(multiplicativeBlock.events.OnSecond);
        }

        combinedBlock.playerStats.health                *= 1 + multBlock.playerStats.health;
        combinedBlock.playerStats.runSpeed              *= 1 + multBlock.playerStats.runSpeed;
        combinedBlock.playerStats.walkSpeed             *= 1 + multBlock.playerStats.walkSpeed;
        combinedBlock.playerStats.totalVision           *= 1 + multBlock.playerStats.totalVision;
        combinedBlock.playerStats.visionConeAngle       *= 1 + multBlock.playerStats.visionConeAngle;
        combinedBlock.playerStats.visionConeRadius      *= 1 + multBlock.playerStats.visionConeRadius;
        combinedBlock.playerStats.visionProximityRadius *= 1 + multBlock.playerStats.visionProximityRadius;

        combinedBlock.gunStats.magazineSize     *= 1 + multBlock.gunStats.magazineSize;
        combinedBlock.gunStats.reloadSpeed      *= 1 + multBlock.gunStats.reloadSpeed;
        combinedBlock.gunStats.fireSpeed        *= 1 + multBlock.gunStats.fireSpeed;
        combinedBlock.gunStats.bulletStreams      *= 1 + multBlock.gunStats.bulletStreams;
        combinedBlock.gunStats.bulletsPerShot   *= 1 + multBlock.gunStats.bulletsPerShot;
        combinedBlock.gunStats.spreadAngle      *= 1 + multBlock.gunStats.spreadAngle;
        combinedBlock.gunStats.accuracy         *= 1 + multBlock.gunStats.accuracy;

        combinedBlock.gunStats.damage    *= 1 + multBlock.gunStats.damage;
        combinedBlock.gunStats.speed     *= 1 + multBlock.gunStats.speed;
        combinedBlock.gunStats.size      *= 1 + multBlock.gunStats.size;
        combinedBlock.gunStats.knockback *= 1 + multBlock.gunStats.knockback;
        combinedBlock.gunStats.bounce    *= 1 + multBlock.gunStats.bounce;
        combinedBlock.gunStats.pierce    *= 1 + multBlock.gunStats.pierce;
        combinedBlock.gunStats.lifetime *= 1 + multBlock.gunStats.lifetime;

        combinedBlock.events.OnFire.AddRange(multBlock.events.OnFire);
        combinedBlock.events.OnHit.AddRange(multBlock.events.OnHit);
        combinedBlock.events.OnKill.AddRange(multBlock.events.OnKill);
        combinedBlock.events.OnReload.AddRange(multBlock.events.OnReload);
        combinedBlock.events.OnSecond.AddRange(multBlock.events.OnSecond);

        return combinedBlock;
    }
}
