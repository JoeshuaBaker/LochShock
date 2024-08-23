using System;
using UnityEngine;

[CreateAssetMenu(fileName = "IsDamageType", menuName = "NemesisShock/Conditions/ItemPropertiesConditions/IsDamageType")]
public class IsDamageType : ItemPropertiesCondition
{
    public enum GunOrActiveItem
    {
        Gun,
        ActiveItem,
        Either
    }

    public enum DamageTypeMatch
    {
        Partial,
        Exact
    }
    public DamageType damageType;
    public DamageTypeMatch matchType = DamageTypeMatch.Partial;
    public GunOrActiveItem mode = GunOrActiveItem.Gun;

    public override float CheckCondition(GameContext context)
    {
        if (base.CheckCondition(context) == 0f || damageType == DamageType.None)
        {
            return 0f;
        }

        DamageType contextDamageType = DamageType.None;
        if (context.damageContext.damageType != DamageType.None)
        {
            contextDamageType = context.damageContext.damageType;
        }
        else if (mode == GunOrActiveItem.Gun || mode == GunOrActiveItem.Either)
        {
            contextDamageType = context.player.inventory.activeGun.damageType;
        }
        else if(mode == GunOrActiveItem.ActiveItem || mode == GunOrActiveItem.Either)
        {
            if(context.player.inventory.activeItem != null)
            {
                contextDamageType = context.player.inventory.activeItem.damageType;
            }
        }

        float stacks = 0f;

        if(matchType == DamageTypeMatch.Partial)
        {
            foreach(DamageType oneType in Enum.GetValues(typeof(DamageType)))
            {
                if(oneType != DamageType.None && damageType.HasFlag(oneType) && contextDamageType.HasFlag(oneType))
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
        }

        return stacks;
    }
}
