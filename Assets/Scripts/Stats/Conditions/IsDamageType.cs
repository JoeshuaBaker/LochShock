using System;
using UnityEngine;

[CreateAssetMenu(fileName = "IsDamageType", menuName = "NemesisShock/Conditions/ItemPropertiesConditions/IsDamageType")]
public class IsDamageType : ItemPropertiesCondition
{
    public enum DamageTypeMatch
    {
        Partial,
        Exact
    }
    public DamageType damageType;
    public DamageTypeMatch matchType = DamageTypeMatch.Partial;

    public override float CheckCondition(GameContext context)
    {
        if (base.CheckCondition(context) == 0f || damageType == DamageType.None)
        {
            return 0f;
        }

        DamageType contextDamageType = DamageType.None;
        DamageType gunDamageType = DamageType.None;
        DamageType activeItemDamageType = DamageType.None;

        if (context.damageContext.damageType != DamageType.None)
        {
            contextDamageType = context.damageContext.damageType;
        }
        if (mode == GunOrActiveItem.Gun || mode == GunOrActiveItem.Either)
        {
            gunDamageType |= context.player.inventory.activeGun.damageType;
        }
        if(mode == GunOrActiveItem.ActiveItem || mode == GunOrActiveItem.Either)
        {
            activeItemDamageType |= context.player.inventory.activeItem.damageType;
        }

        float stacks = 0f;

        if(matchType == DamageTypeMatch.Partial)
        {
            foreach(DamageType oneType in Enum.GetValues(typeof(DamageType)))
            {
                if (oneType != DamageType.None && damageType.HasFlag(oneType) && contextDamageType.HasFlag(oneType))
                {
                    stacks = 1;
                    break;
                }
                if ((mode == GunOrActiveItem.Gun || mode == GunOrActiveItem.Either) && oneType != DamageType.None && damageType.HasFlag(oneType) && gunDamageType.HasFlag(oneType))
                {
                    stacks = 1;
                    break;
                }
                if ((mode == GunOrActiveItem.ActiveItem || mode == GunOrActiveItem.Either) && oneType != DamageType.None && damageType.HasFlag(oneType) && activeItemDamageType.HasFlag(oneType))
                {
                    stacks = 1;
                    break;
                }
            }
        }
        else if(matchType == DamageTypeMatch.Exact)
        {
            if(damageType.HasFlag(contextDamageType))
            {
                stacks = 1;
            }
            if((mode == GunOrActiveItem.Gun || mode == GunOrActiveItem.Either) && damageType.HasFlag(gunDamageType))
            {
                stacks = 1;
            }
            if((mode == GunOrActiveItem.Gun || mode == GunOrActiveItem.Either) && damageType.HasFlag(activeItemDamageType))
            {
                stacks = 1;
            }
        }

        return stacks;
    }
    public override string ConditionTooltipValueLabelInsert => damageType.ToString().Replace(",", (matchType == DamageTypeMatch.Exact) ? "" : " and").ToLower();
}
