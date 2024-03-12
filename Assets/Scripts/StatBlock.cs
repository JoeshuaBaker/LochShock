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
    public BulletStats bulletStats = new BulletStats();
    
    [Serializable]
    public class PlayerStats
    {
        public BlockType blockType;
        public float health;
        public float walkSpeed;
        public float runSpeed;
    }

    [Serializable]
    public class GunStats
    {
        public BlockType blockType;
        public float magazineSize;
        public float reloadSpeed;
        public float fireSpeed;
        public float bulletsPerShot;
        public float bulletSpread;
        public List<Action> OnFire = new List<Action>();
    }

    [Serializable]
    public class BulletStats
    {
        public BlockType blockType;
        public float damage;
        public float speed;
        public float size;
        public float knockback;
        public float bounce;
        public float pierce;
        public float lifetime;
        public List<Action> OnHit = new List<Action>();
        public List<Action> OnDestroy = new List<Action>();
    }

    public static StatBlock Combine(IEnumerable<StatBlock> blocks)
    {
        StatBlock combinedBlock = blocks.FirstOrDefault(x => x.blockType == BlockType.Base) ?? new StatBlock();
        IEnumerable<IGrouping<BlockType, StatBlock>> groupedBlocks = blocks.GroupBy(x => x.blockType);
        foreach(StatBlock additiveBlock in groupedBlocks.Where(x => x.Key == BlockType.Additive))
        {
            combinedBlock.playerStats.health    += additiveBlock.playerStats.health;
            combinedBlock.playerStats.runSpeed  += additiveBlock.playerStats.runSpeed;
            combinedBlock.playerStats.walkSpeed += additiveBlock.playerStats.walkSpeed;

            combinedBlock.gunStats.magazineSize += additiveBlock.gunStats.magazineSize;
            combinedBlock.gunStats.reloadSpeed += additiveBlock.gunStats.reloadSpeed;
            combinedBlock.gunStats.fireSpeed += additiveBlock.gunStats.fireSpeed;
            combinedBlock.gunStats.bulletsPerShot += additiveBlock.gunStats.bulletsPerShot;
            combinedBlock.gunStats.bulletSpread += additiveBlock.gunStats.bulletSpread;
            combinedBlock.gunStats.OnFire.AddRange(additiveBlock.gunStats.OnFire);

            combinedBlock.bulletStats.damage += additiveBlock.bulletStats.damage;
            combinedBlock.bulletStats.speed += additiveBlock.bulletStats.speed;
            combinedBlock.bulletStats.size += additiveBlock.bulletStats.size;
            combinedBlock.bulletStats.knockback += additiveBlock.bulletStats.knockback;
            combinedBlock.bulletStats.bounce += additiveBlock.bulletStats.bounce;
            combinedBlock.bulletStats.pierce += additiveBlock.bulletStats.pierce;
            combinedBlock.bulletStats.lifetime += additiveBlock.bulletStats.lifetime;
            combinedBlock.bulletStats.OnHit.AddRange(additiveBlock.bulletStats.OnHit);
            combinedBlock.bulletStats.OnDestroy.AddRange(additiveBlock.bulletStats.OnDestroy);
        }

        //Add all multipliers together, then apply them once at the end.
        StatBlock multBlock = new StatBlock();

        foreach (StatBlock multiplicativeBlock in groupedBlocks.Where(x => x.Key == BlockType.Multiplicative))
        {
            multBlock.playerStats.health += multiplicativeBlock.playerStats.health;
            multBlock.playerStats.runSpeed += multiplicativeBlock.playerStats.runSpeed;
            multBlock.playerStats.walkSpeed += multiplicativeBlock.playerStats.walkSpeed;

            multBlock.gunStats.magazineSize += multiplicativeBlock.gunStats.magazineSize;
            multBlock.gunStats.reloadSpeed += multiplicativeBlock.gunStats.reloadSpeed;
            multBlock.gunStats.fireSpeed += multiplicativeBlock.gunStats.fireSpeed;
            multBlock.gunStats.bulletsPerShot += multiplicativeBlock.gunStats.bulletsPerShot;
            multBlock.gunStats.bulletSpread += multiplicativeBlock.gunStats.bulletSpread;
            multBlock.gunStats.OnFire.AddRange(multiplicativeBlock.gunStats.OnFire);

            multBlock.bulletStats.damage += multiplicativeBlock.bulletStats.damage;
            multBlock.bulletStats.speed += multiplicativeBlock.bulletStats.speed;
            multBlock.bulletStats.size += multiplicativeBlock.bulletStats.size;
            multBlock.bulletStats.knockback += multiplicativeBlock.bulletStats.knockback;
            multBlock.bulletStats.bounce += multiplicativeBlock.bulletStats.bounce;
            multBlock.bulletStats.pierce += multiplicativeBlock.bulletStats.pierce;
            multBlock.bulletStats.lifetime += multiplicativeBlock.bulletStats.lifetime;
            multBlock.bulletStats.OnHit.AddRange(multiplicativeBlock.bulletStats.OnHit);
            multBlock.bulletStats.OnDestroy.AddRange(multiplicativeBlock.bulletStats.OnDestroy);
        }

        combinedBlock.playerStats.health    *= 1 + multBlock.playerStats.health;
        combinedBlock.playerStats.runSpeed  *= 1 + multBlock.playerStats.runSpeed;
        combinedBlock.playerStats.walkSpeed *= 1 + multBlock.playerStats.walkSpeed;

        combinedBlock.gunStats.magazineSize     *= 1 + multBlock.gunStats.magazineSize;
        combinedBlock.gunStats.reloadSpeed      *= 1 + multBlock.gunStats.reloadSpeed;
        combinedBlock.gunStats.fireSpeed        *= 1 + multBlock.gunStats.fireSpeed;
        combinedBlock.gunStats.bulletsPerShot   *= 1 + multBlock.gunStats.bulletsPerShot;
        combinedBlock.gunStats.bulletSpread     *= 1 + multBlock.gunStats.bulletSpread;
        combinedBlock.gunStats.OnFire.AddRange(multBlock.gunStats.OnFire);

        combinedBlock.bulletStats.damage    *= 1 + multBlock.bulletStats.damage;
        combinedBlock.bulletStats.speed     *= 1 + multBlock.bulletStats.speed;
        combinedBlock.bulletStats.size      *= 1 + multBlock.bulletStats.size;
        combinedBlock.bulletStats.knockback *= 1 + multBlock.bulletStats.knockback;
        combinedBlock.bulletStats.bounce    *= 1 + multBlock.bulletStats.bounce;
        combinedBlock.bulletStats.pierce    *= 1 + multBlock.bulletStats.pierce;
        combinedBlock.bulletStats.lifetime  *= 1 + multBlock.bulletStats.lifetime;
        combinedBlock.bulletStats.OnHit.AddRange(multBlock.bulletStats.OnHit);
        combinedBlock.bulletStats.OnDestroy.AddRange(multBlock.bulletStats.OnDestroy);

        return combinedBlock;
    }
}
