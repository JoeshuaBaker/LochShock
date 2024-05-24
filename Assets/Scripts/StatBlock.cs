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
        PlusMult, //Multiplicative values should be represented without the 1. E.g 20% = 0.2f, 200% = 2f
        xMult,
        Set,
        Base
    }

    //StatBlock properties
    public BlockType blockType;
    public PlayerStats playerStats = new PlayerStats();
    public GunStats gunStats = new GunStats();
    public Events events = new Events();
    public int stacks = 1;

    //base consts for each stat
    public const float HEALTH_MIN = 0f;
    public const float WALKSPEED_MIN = 0f;
    public const float RUNSPEED_MIN = 1f;
    public const float TOTALVISION_MIN = 0f;
    public const float TOTALVISION_MAX = 1f;
    public const float VISIONCONEANGLE_MIN = 0f;
    public const float VISIONCONEANGLE_MAX = 90f;
    public const float VISIONCONERADIUS_MIN = 0f;
    public const float VISIONPROXIMITYRADIUS_MIN = 0f;

    public const float MAGAZINESIZE_MIN = 1f;
    public const float RELOADSPEED_MIN = 1f/60f;
    public const float FIRESPEED_MIN = 1f/60f;
    public const float BULLETSTREAMS_MIN = 1f;
    public const float BULLETSTREAMS_MAX = 10f;
    public const float BULLETSPERSHOT_MIN = 1f;
    public const float BULLETSPERSHOT_MAX = 25f;
    public const float SPREADANGLE_MIN = 0f;
    public const float SPREADANGLE_MAX = 90f;
    public const float ACCURACY_MIN = 0f;
    public const float ACCURACY_MAX = 1f;
    public const float DAMAGE_MIN = 0f;
    public const float VELOCITY_MIN = 1f;
    public const float VELOCITY_MAX = 300f;
    public const float SIZE_MIN = 0.01f;
    public const float SIZE_MAX = 5f;
    public const float KNOCKBACK_MIN = 0f;
    public const float BOUNCE_MIN = 0f;
    public const float PIERCE_MIN = 0f;
    public const float LIFETIME_MIN = 0.05f;
    public const float LIFETIME_MAX = 5f;

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
        public float velocity;
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
        StatBlock copy = new StatBlock(this.blockType);
        copy.playerStats = this.playerStats;
        copy.gunStats = this.gunStats;
        copy.stacks = this.stacks;
        copy.events = events.Copy();
        
        return copy;
    }

    public static StatBlock Combine(IEnumerable<StatBlock> blocks)
    {
        //Sort blocks by type
        StatBlock combinedBlock = blocks.FirstOrDefault(x => x.blockType == BlockType.Base).Copy() ?? new StatBlock(BlockType.Base);
        List<StatBlock> additiveBlocks  = new List<StatBlock>();
        List<StatBlock> multBlocks      = new List<StatBlock>();
        List<StatBlock> xMultblocks     = new List<StatBlock>();
        List<StatBlock> setBlocks       = new List<StatBlock>();

        //Sort all blocks into their respective groups by type
        foreach(var block in blocks)
        {
            switch (block.blockType)
            {
                case BlockType.Additive:
                    additiveBlocks.Add(block);
                    break;

                case BlockType.PlusMult:
                    multBlocks.Add(block);
                    break;

                case BlockType.xMult:
                    xMultblocks.Add(block);
                    break;

                case BlockType.Set:
                    setBlocks.Add(block);
                    break;
            }

            //all events are additive
            combinedBlock.events.OnFire.AddRange(block.events.OnFire);
            combinedBlock.events.OnHit.AddRange(block.events.OnHit);
            combinedBlock.events.OnKill.AddRange(block.events.OnKill);
            combinedBlock.events.OnReload.AddRange(block.events.OnReload);
            combinedBlock.events.OnSecond.AddRange(block.events.OnSecond);
        }

        if(additiveBlocks.Count > 0)
        {
            //Add together additive blocks
            foreach (StatBlock additiveBlock in additiveBlocks)
            {
                combinedBlock.playerStats.health += additiveBlock.playerStats.health * additiveBlock.stacks;
                combinedBlock.playerStats.runSpeed += additiveBlock.playerStats.runSpeed * additiveBlock.stacks;
                combinedBlock.playerStats.walkSpeed += additiveBlock.playerStats.walkSpeed * additiveBlock.stacks;
                combinedBlock.playerStats.totalVision += additiveBlock.playerStats.totalVision * additiveBlock.stacks;
                combinedBlock.playerStats.visionConeAngle += additiveBlock.playerStats.visionConeAngle * additiveBlock.stacks;
                combinedBlock.playerStats.visionConeRadius += additiveBlock.playerStats.visionConeRadius * additiveBlock.stacks;
                combinedBlock.playerStats.visionProximityRadius += additiveBlock.playerStats.visionProximityRadius * additiveBlock.stacks;

                combinedBlock.gunStats.magazineSize += additiveBlock.gunStats.magazineSize * additiveBlock.stacks;
                combinedBlock.gunStats.reloadSpeed += additiveBlock.gunStats.reloadSpeed * additiveBlock.stacks;
                combinedBlock.gunStats.fireSpeed += additiveBlock.gunStats.fireSpeed * additiveBlock.stacks;
                combinedBlock.gunStats.bulletStreams += additiveBlock.gunStats.bulletStreams * additiveBlock.stacks;
                combinedBlock.gunStats.bulletsPerShot += additiveBlock.gunStats.bulletsPerShot * additiveBlock.stacks;
                combinedBlock.gunStats.spreadAngle += additiveBlock.gunStats.spreadAngle * additiveBlock.stacks;
                combinedBlock.gunStats.accuracy += additiveBlock.gunStats.accuracy * additiveBlock.stacks;

                combinedBlock.gunStats.damage += additiveBlock.gunStats.damage * additiveBlock.stacks;
                combinedBlock.gunStats.velocity += additiveBlock.gunStats.velocity * additiveBlock.stacks;
                combinedBlock.gunStats.size += additiveBlock.gunStats.size * additiveBlock.stacks;
                combinedBlock.gunStats.knockback += additiveBlock.gunStats.knockback * additiveBlock.stacks;
                combinedBlock.gunStats.bounce += additiveBlock.gunStats.bounce * additiveBlock.stacks;
                combinedBlock.gunStats.pierce += additiveBlock.gunStats.pierce * additiveBlock.stacks;
                combinedBlock.gunStats.lifetime += additiveBlock.gunStats.lifetime * additiveBlock.stacks;
            }
        }

        //PlusMult: Add together all blocks, then multiply once at the end.
        if (multBlocks.Count > 0)
        {
            StatBlock multBlock = new StatBlock(BlockType.PlusMult);

            foreach (StatBlock multiplicativeBlock in multBlocks)
            {
                multBlock.playerStats.health += multiplicativeBlock.playerStats.health * multiplicativeBlock.stacks;
                multBlock.playerStats.runSpeed += multiplicativeBlock.playerStats.runSpeed * multiplicativeBlock.stacks;
                multBlock.playerStats.walkSpeed += multiplicativeBlock.playerStats.walkSpeed * multiplicativeBlock.stacks;
                multBlock.playerStats.totalVision += multiplicativeBlock.playerStats.totalVision * multiplicativeBlock.stacks;
                multBlock.playerStats.visionConeAngle += multiplicativeBlock.playerStats.visionConeAngle * multiplicativeBlock.stacks;
                multBlock.playerStats.visionConeRadius += multiplicativeBlock.playerStats.visionConeRadius * multiplicativeBlock.stacks;
                multBlock.playerStats.visionProximityRadius += multiplicativeBlock.playerStats.visionProximityRadius * multiplicativeBlock.stacks;

                multBlock.gunStats.magazineSize += multiplicativeBlock.gunStats.magazineSize * multiplicativeBlock.stacks;
                multBlock.gunStats.reloadSpeed += multiplicativeBlock.gunStats.reloadSpeed * multiplicativeBlock.stacks;
                multBlock.gunStats.fireSpeed += multiplicativeBlock.gunStats.fireSpeed * multiplicativeBlock.stacks;
                multBlock.gunStats.bulletStreams += multiplicativeBlock.gunStats.bulletStreams * multiplicativeBlock.stacks;
                multBlock.gunStats.bulletsPerShot += multiplicativeBlock.gunStats.bulletsPerShot * multiplicativeBlock.stacks;
                multBlock.gunStats.spreadAngle += multiplicativeBlock.gunStats.spreadAngle * multiplicativeBlock.stacks;
                multBlock.gunStats.accuracy += multiplicativeBlock.gunStats.accuracy * multiplicativeBlock.stacks;

                multBlock.gunStats.damage += multiplicativeBlock.gunStats.damage * multiplicativeBlock.stacks;
                multBlock.gunStats.velocity += multiplicativeBlock.gunStats.velocity * multiplicativeBlock.stacks;
                multBlock.gunStats.size += multiplicativeBlock.gunStats.size * multiplicativeBlock.stacks;
                multBlock.gunStats.knockback += multiplicativeBlock.gunStats.knockback * multiplicativeBlock.stacks;
                multBlock.gunStats.bounce += multiplicativeBlock.gunStats.bounce * multiplicativeBlock.stacks;
                multBlock.gunStats.pierce += multiplicativeBlock.gunStats.pierce * multiplicativeBlock.stacks;
                multBlock.gunStats.lifetime += multiplicativeBlock.gunStats.lifetime * multiplicativeBlock.stacks;
            }

            combinedBlock.playerStats.health *= 1 + multBlock.playerStats.health;
            combinedBlock.playerStats.runSpeed *= 1 + multBlock.playerStats.runSpeed;
            combinedBlock.playerStats.walkSpeed *= 1 + multBlock.playerStats.walkSpeed;
            combinedBlock.playerStats.totalVision *= 1 + multBlock.playerStats.totalVision;
            combinedBlock.playerStats.visionConeAngle *= 1 + multBlock.playerStats.visionConeAngle;
            combinedBlock.playerStats.visionConeRadius *= 1 + multBlock.playerStats.visionConeRadius;
            combinedBlock.playerStats.visionProximityRadius *= 1 + multBlock.playerStats.visionProximityRadius;

            combinedBlock.gunStats.magazineSize *= 1 + multBlock.gunStats.magazineSize;
            combinedBlock.gunStats.reloadSpeed *= 1 + multBlock.gunStats.reloadSpeed;
            combinedBlock.gunStats.fireSpeed *= 1 + multBlock.gunStats.fireSpeed;
            combinedBlock.gunStats.bulletStreams *= 1 + multBlock.gunStats.bulletStreams;
            combinedBlock.gunStats.bulletsPerShot *= 1 + multBlock.gunStats.bulletsPerShot;
            combinedBlock.gunStats.spreadAngle *= 1 + multBlock.gunStats.spreadAngle;
            combinedBlock.gunStats.accuracy *= 1 + multBlock.gunStats.accuracy;

            combinedBlock.gunStats.damage *= 1 + multBlock.gunStats.damage;
            combinedBlock.gunStats.velocity *= 1 + multBlock.gunStats.velocity;
            combinedBlock.gunStats.size *= 1 + multBlock.gunStats.size;
            combinedBlock.gunStats.knockback *= 1 + multBlock.gunStats.knockback;
            combinedBlock.gunStats.bounce *= 1 + multBlock.gunStats.bounce;
            combinedBlock.gunStats.pierce *= 1 + multBlock.gunStats.pierce;
            combinedBlock.gunStats.lifetime *= 1 + multBlock.gunStats.lifetime;
        }

        if (xMultblocks.Count > 0)
        {
            //xMult: Multiply value by each block value.
            foreach (var xMultBlock in xMultblocks)
            {
                combinedBlock.playerStats.health *= 1 + xMultBlock.playerStats.health * xMultBlock.stacks;
                combinedBlock.playerStats.runSpeed *= 1 + xMultBlock.playerStats.runSpeed * xMultBlock.stacks;
                combinedBlock.playerStats.walkSpeed *= 1 + xMultBlock.playerStats.walkSpeed * xMultBlock.stacks;
                combinedBlock.playerStats.totalVision *= 1 + xMultBlock.playerStats.totalVision * xMultBlock.stacks;
                combinedBlock.playerStats.visionConeAngle *= 1 + xMultBlock.playerStats.visionConeAngle * xMultBlock.stacks;
                combinedBlock.playerStats.visionConeRadius *= 1 + xMultBlock.playerStats.visionConeRadius * xMultBlock.stacks;
                combinedBlock.playerStats.visionProximityRadius *= 1 + xMultBlock.playerStats.visionProximityRadius * xMultBlock.stacks;

                combinedBlock.gunStats.magazineSize *= 1 + xMultBlock.gunStats.magazineSize * xMultBlock.stacks;
                combinedBlock.gunStats.reloadSpeed *= 1 + xMultBlock.gunStats.reloadSpeed * xMultBlock.stacks;
                combinedBlock.gunStats.fireSpeed *= 1 + xMultBlock.gunStats.fireSpeed * xMultBlock.stacks;
                combinedBlock.gunStats.bulletStreams *= 1 + xMultBlock.gunStats.bulletStreams * xMultBlock.stacks;
                combinedBlock.gunStats.bulletsPerShot *= 1 + xMultBlock.gunStats.bulletsPerShot * xMultBlock.stacks;
                combinedBlock.gunStats.spreadAngle *= 1 + xMultBlock.gunStats.spreadAngle * xMultBlock.stacks;
                combinedBlock.gunStats.accuracy *= 1 + xMultBlock.gunStats.accuracy * xMultBlock.stacks;

                combinedBlock.gunStats.damage *= 1 + xMultBlock.gunStats.damage * xMultBlock.stacks;
                combinedBlock.gunStats.velocity *= 1 + xMultBlock.gunStats.velocity * xMultBlock.stacks;
                combinedBlock.gunStats.size *= 1 + xMultBlock.gunStats.size * xMultBlock.stacks;
                combinedBlock.gunStats.knockback *= 1 + xMultBlock.gunStats.knockback * xMultBlock.stacks;
                combinedBlock.gunStats.bounce *= 1 + xMultBlock.gunStats.bounce * xMultBlock.stacks;
                combinedBlock.gunStats.pierce *= 1 + xMultBlock.gunStats.pierce * xMultBlock.stacks;
                combinedBlock.gunStats.lifetime *= 1 + xMultBlock.gunStats.lifetime* xMultBlock.stacks;
            }
        }


        if (setBlocks.Count > 0)
        {
            //SetBlocks: Set value to a specific range. Defer to lesser of the two values if two setblocks conflict
            StatBlock setBlockCombined = new StatBlock(BlockType.Set);

            //don't ever set value to 0. if both set blocks have values, take the min.
            Func<float, float, float> setCombine = (sb1, sb2) =>
            {
                return sb1 == 0 ? sb2 : sb2 == 0 ? sb1 : Mathf.Min(sb1, sb2);
            };

            foreach (var setBlock in setBlocks)
            {
                setBlockCombined.playerStats.health = setCombine(setBlockCombined.playerStats.health, setBlock.playerStats.health);
                setBlockCombined.playerStats.runSpeed = setCombine(setBlockCombined.playerStats.runSpeed, setBlock.playerStats.runSpeed);
                setBlockCombined.playerStats.walkSpeed = setCombine(setBlockCombined.playerStats.walkSpeed, setBlock.playerStats.walkSpeed);
                setBlockCombined.playerStats.totalVision = setCombine(setBlockCombined.playerStats.totalVision, setBlock.playerStats.totalVision);
                setBlockCombined.playerStats.visionConeAngle = setCombine(setBlockCombined.playerStats.visionConeAngle, setBlock.playerStats.visionConeAngle);
                setBlockCombined.playerStats.visionConeRadius = setCombine(setBlockCombined.playerStats.visionConeRadius, setBlock.playerStats.visionConeRadius);
                setBlockCombined.playerStats.visionProximityRadius = setCombine(setBlockCombined.playerStats.visionProximityRadius, setBlock.playerStats.visionProximityRadius);

                setBlockCombined.gunStats.magazineSize = setCombine(setBlockCombined.gunStats.magazineSize, setBlock.gunStats.magazineSize);
                setBlockCombined.gunStats.reloadSpeed = setCombine(setBlockCombined.gunStats.reloadSpeed, setBlock.gunStats.reloadSpeed);
                setBlockCombined.gunStats.fireSpeed = setCombine(setBlockCombined.gunStats.fireSpeed, setBlock.gunStats.fireSpeed);
                setBlockCombined.gunStats.bulletStreams = setCombine(setBlockCombined.gunStats.bulletStreams, setBlock.gunStats.bulletStreams);
                setBlockCombined.gunStats.bulletsPerShot = setCombine(setBlockCombined.gunStats.bulletsPerShot, setBlock.gunStats.bulletsPerShot);
                setBlockCombined.gunStats.spreadAngle = setCombine(setBlockCombined.gunStats.spreadAngle, setBlock.gunStats.spreadAngle);
                setBlockCombined.gunStats.accuracy = setCombine(setBlockCombined.gunStats.accuracy, setBlock.gunStats.accuracy);

                setBlockCombined.gunStats.damage = setCombine(setBlockCombined.gunStats.damage, setBlock.gunStats.damage);
                setBlockCombined.gunStats.velocity = setCombine(setBlockCombined.gunStats.velocity, setBlock.gunStats.velocity);
                setBlockCombined.gunStats.size = setCombine(setBlockCombined.gunStats.size, setBlock.gunStats.size);
                setBlockCombined.gunStats.knockback = setCombine(setBlockCombined.gunStats.knockback, setBlock.gunStats.knockback);
                setBlockCombined.gunStats.bounce = setCombine(setBlockCombined.gunStats.bounce, setBlock.gunStats.bounce);
                setBlockCombined.gunStats.pierce = setCombine(setBlockCombined.gunStats.pierce, setBlock.gunStats.pierce);
                setBlockCombined.gunStats.lifetime = setCombine(setBlockCombined.gunStats.lifetime, setBlock.gunStats.lifetime);
            }

            Func<float, float, float> set = (sb1, sb2) =>
            {
                return sb2 == 0 ? sb1 : sb2;
            };

            combinedBlock.playerStats.health = set(combinedBlock.playerStats.health, setBlockCombined.playerStats.health);
            combinedBlock.playerStats.runSpeed = set(combinedBlock.playerStats.runSpeed, setBlockCombined.playerStats.runSpeed);
            combinedBlock.playerStats.walkSpeed = set(combinedBlock.playerStats.walkSpeed, setBlockCombined.playerStats.walkSpeed);
            combinedBlock.playerStats.totalVision = set(combinedBlock.playerStats.totalVision, setBlockCombined.playerStats.totalVision);
            combinedBlock.playerStats.visionConeAngle = set(combinedBlock.playerStats.visionConeAngle, setBlockCombined.playerStats.visionConeAngle);
            combinedBlock.playerStats.visionConeRadius = set(combinedBlock.playerStats.visionConeRadius, setBlockCombined.playerStats.visionConeRadius);
            combinedBlock.playerStats.visionProximityRadius = set(combinedBlock.playerStats.visionProximityRadius, setBlockCombined.playerStats.visionProximityRadius);

            combinedBlock.gunStats.magazineSize = set(combinedBlock.gunStats.magazineSize, setBlockCombined.gunStats.magazineSize);
            combinedBlock.gunStats.reloadSpeed = set(combinedBlock.gunStats.reloadSpeed, setBlockCombined.gunStats.reloadSpeed);
            combinedBlock.gunStats.fireSpeed = set(combinedBlock.gunStats.fireSpeed, setBlockCombined.gunStats.fireSpeed);
            combinedBlock.gunStats.bulletStreams = set(combinedBlock.gunStats.bulletStreams, setBlockCombined.gunStats.bulletStreams);
            combinedBlock.gunStats.bulletsPerShot = set(combinedBlock.gunStats.bulletsPerShot, setBlockCombined.gunStats.bulletsPerShot);
            combinedBlock.gunStats.spreadAngle = set(combinedBlock.gunStats.spreadAngle, setBlockCombined.gunStats.spreadAngle);
            combinedBlock.gunStats.accuracy = set(combinedBlock.gunStats.accuracy, setBlockCombined.gunStats.accuracy);

            combinedBlock.gunStats.damage = set(combinedBlock.gunStats.damage, setBlockCombined.gunStats.damage);
            combinedBlock.gunStats.velocity = set(combinedBlock.gunStats.velocity, setBlockCombined.gunStats.velocity);
            combinedBlock.gunStats.size = set(combinedBlock.gunStats.size, setBlockCombined.gunStats.size);
            combinedBlock.gunStats.knockback = set(combinedBlock.gunStats.knockback, setBlockCombined.gunStats.knockback);
            combinedBlock.gunStats.bounce = set(combinedBlock.gunStats.bounce, setBlockCombined.gunStats.bounce);
            combinedBlock.gunStats.pierce = set(combinedBlock.gunStats.pierce, setBlockCombined.gunStats.pierce);
            combinedBlock.gunStats.lifetime = set(combinedBlock.gunStats.lifetime, setBlockCombined.gunStats.lifetime);
        }

        //Clamp values that should have a min/max
        combinedBlock.playerStats.health = Mathf.Max(combinedBlock.playerStats.health, HEALTH_MIN);
        combinedBlock.playerStats.runSpeed = Mathf.Max(combinedBlock.playerStats.runSpeed, RUNSPEED_MIN);
        combinedBlock.playerStats.walkSpeed = Mathf.Max(combinedBlock.playerStats.walkSpeed, WALKSPEED_MIN);
        combinedBlock.playerStats.totalVision = Mathf.Clamp(combinedBlock.playerStats.totalVision, TOTALVISION_MIN, TOTALVISION_MAX);
        combinedBlock.playerStats.visionConeAngle = Mathf.Clamp(combinedBlock.playerStats.visionConeAngle, VISIONCONEANGLE_MIN, VISIONCONEANGLE_MAX);
        combinedBlock.playerStats.visionConeRadius = Mathf.Max(combinedBlock.playerStats.visionConeRadius, VISIONCONERADIUS_MIN);
        combinedBlock.playerStats.visionProximityRadius = Mathf.Max(combinedBlock.playerStats.visionProximityRadius, VISIONPROXIMITYRADIUS_MIN);

        combinedBlock.gunStats.magazineSize     = Mathf.Max(combinedBlock.gunStats.magazineSize, MAGAZINESIZE_MIN);
        combinedBlock.gunStats.reloadSpeed      = Mathf.Max(combinedBlock.gunStats.reloadSpeed, RELOADSPEED_MIN);
        combinedBlock.gunStats.fireSpeed        = Mathf.Max(combinedBlock.gunStats.fireSpeed, FIRESPEED_MIN);
        combinedBlock.gunStats.bulletStreams    = Mathf.Clamp(combinedBlock.gunStats.bulletStreams, BULLETSTREAMS_MIN, BULLETSTREAMS_MAX);
        combinedBlock.gunStats.bulletsPerShot   = Mathf.Clamp(combinedBlock.gunStats.bulletsPerShot, BULLETSPERSHOT_MIN, BULLETSPERSHOT_MAX);
        combinedBlock.gunStats.spreadAngle      = Mathf.Clamp(combinedBlock.gunStats.spreadAngle, SPREADANGLE_MIN, SPREADANGLE_MAX);
        combinedBlock.gunStats.accuracy         = Mathf.Clamp(combinedBlock.gunStats.accuracy, ACCURACY_MIN, ACCURACY_MAX);

        combinedBlock.gunStats.damage       = Mathf.Max(combinedBlock.gunStats.damage, DAMAGE_MIN);
        combinedBlock.gunStats.velocity     = Mathf.Clamp(combinedBlock.gunStats.velocity, VELOCITY_MIN, VELOCITY_MAX);
        combinedBlock.gunStats.size         = Mathf.Clamp(combinedBlock.gunStats.size, SIZE_MIN, SIZE_MAX);
        combinedBlock.gunStats.knockback    = Mathf.Max(combinedBlock.gunStats.knockback, KNOCKBACK_MIN);
        combinedBlock.gunStats.bounce       = Mathf.Max(combinedBlock.gunStats.bounce, BOUNCE_MIN);
        combinedBlock.gunStats.pierce       = Mathf.Max(combinedBlock.gunStats.pierce, PIERCE_MIN);
        combinedBlock.gunStats.lifetime     = Mathf.Clamp(combinedBlock.gunStats.lifetime, LIFETIME_MIN, LIFETIME_MAX);

        return combinedBlock;
    }
}
