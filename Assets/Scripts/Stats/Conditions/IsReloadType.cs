using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IsReloadType", menuName = "NemesisShock/Conditions/ItemPropertiesConditions/IsReloadType")]
public class IsReloadType : ItemPropertiesCondition
{
    public ReloadType reloadType;
    public override float CheckCondition(GameContext context)
    {
        if (base.CheckCondition(context) == 0f)
        {
            return 0f;
        }

        ReloadType contextReloadType = context.player.inventory.activeGun.reloadType;

        return contextReloadType == reloadType ? 1f : 0f;
    }

    public override string ConditionTooltipLabel => "if";
    public override string ConditionTooltipPostfix => ConditionTooltipLabel + $" reload type is {reloadType.ToString().ToLower()}";
}
