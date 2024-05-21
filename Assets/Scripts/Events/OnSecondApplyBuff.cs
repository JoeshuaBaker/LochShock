using UnityEngine;

[CreateAssetMenu(fileName = "OnSecondApplyBuff", menuName = "NemesisShock/Events/OnSecond/ApplyBuff")]
public class OnSecondApplyBuff : OnSecondAction
{
    public Buff buff;
    public override string GetTooltip(StatBlock stats)
    {
        return $"{(int)(chanceToTrigger * 100f)}% chance to gain {(int)(buff.stats.gunStats.fireSpeed * 100f)}% fire speed for {buff.duration} seconds. " +
            $"(New Fire Rate: {stats.gunStats.fireSpeed * (1f + buff.stats.gunStats.fireSpeed)}).";
    }

    public override void OnSecond(Player player)
    {
        if (IsValidTrigger())
        {
            base.OnSecond(player);
            player.AddBuff(buff.GetInstance());
        }
    }
}
