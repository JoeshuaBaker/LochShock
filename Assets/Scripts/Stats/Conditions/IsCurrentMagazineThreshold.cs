using UnityEngine;

[CreateAssetMenu(fileName = "IsCurrentMagazineThreshold", menuName = "NemesisShock/Conditions/ItemPropertiesConditions/Magazine")]
public class IsCurrentMagazineThreshold : ItemPropertiesCondition
{
    public enum MagazineThreshold
    {
        Greater_Than,
        Less_Than,
        LastBullet,
        Full,
        Empty
    }
    [Range(0,1)] public float currentMagazineThresholdPercent = 1f;
    private string currentMagazineString => (threshold == MagazineThreshold.Empty || threshold == MagazineThreshold.Full) ? "" : currentMagazineThresholdPercent.ToString() + " percent full"; 
    public MagazineThreshold threshold = MagazineThreshold.Empty;

    public override float CheckCondition(GameContext context)
    {
        mode = GunOrActiveItem.Gun;
        if (base.CheckCondition(context) == 0f)
        {
            return 0f;
        }

        int currentMagazine = context.damageContext.bulletInMag;
        int maxMagazine = context.damageContext.maxBulletsInMag;

        if(currentMagazine == 0)
        {
            currentMagazine = Player.activePlayer.inventory.activeGun.magazine;
        }
        if(maxMagazine == 0)
        {
            maxMagazine = Player.activePlayer.inventory.activeGun.maxMagazine;
        }

        float magazinePercent = currentMagazine / (float)maxMagazine;

        if (threshold == MagazineThreshold.Greater_Than)
        {
            return magazinePercent > currentMagazineThresholdPercent ? 1f : 0f;
        }
        else if(threshold == MagazineThreshold.Less_Than)
        {
            return magazinePercent < currentMagazineThresholdPercent ? 1f : 0f;
        }
        else if(threshold == MagazineThreshold.LastBullet)
        {
            return currentMagazine == 1 ? 1f : 0f;
        }
        else if(threshold == MagazineThreshold.Full)
        {
            return maxMagazine == currentMagazine ? 1f : 0f;
        }
        else if(threshold == MagazineThreshold.Empty)
        {
            return currentMagazine == 0 ? 1f : 0f;
        }

        return 0f;
    }

    public override string ConditionTooltipLabel => "when";
    public override string ConditionTooltipPostfix => ConditionTooltipLabel + $" magazine is {threshold.ToString().Replace("_", " ")} {currentMagazineString}";
}