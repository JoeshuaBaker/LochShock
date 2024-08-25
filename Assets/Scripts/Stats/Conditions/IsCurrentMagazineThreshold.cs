using UnityEngine;

[CreateAssetMenu(fileName = "IsCurrentMagazineThreshold", menuName = "NemesisShock/Conditions/ItemPropertiesConditions/IsMagazineEmpty")]
public class IsCurrentMagazineThreshold : ItemPropertiesCondition
{
    public enum MagazineThreshold
    {
        Greater_Than,
        Less_Than,
        Full,
        Empty
    }
    [Range(0,1)] public float currentMagazineThresholdPercent = 1f;
    private string currentMagazineString => (threshold)
    public MagazineThreshold threshold = MagazineThreshold.Empty;

    public override float CheckCondition(GameContext context)
    {
        if (base.CheckCondition(context) == 0f)
        {
            return 0f;
        }

        int currentMagazine = context.damageContext.bulletInMag;
        int maxMagazine = context.damageContext.maxBulletsInMag;

        if(currentMagazine == 0)
        {
            currentMagazine = Player.activePlayer.inventory.activeGun;
        }

        float magazinePercent = context.player.inventory.activeGun.percentMagFilled;

        if (threshold == MagazineThreshold.Greater_Than)
        {
            return currentMagazine > currentMagazineThreshold ? 1f : 0f;
        }
        else
        {
            return currentMagazine < currentMagazineThreshold ? 1f : 0f;
        }
    }

    public override string ConditionTooltipLabel => "when";
    public override string ConditionTooltipPostfix => ConditionTooltipLabel + $"Magazine is {threshold.ToString().Replace("_", " ")} {currentMagazineThreshold}";
}
