using UnityEngine;

[CreateAssetMenu(fileName = "OnFireApplyBuff", menuName = "NemesisShock/Events/OnFire/ApplyBuff")]
public class OnFireApplyBuff : OnFireAction
{
    public Buff buff;
    public override string GetTooltip(StatBlock stats)
    {
        return $"{(int)(chanceToTrigger * 100f)}% chance to gain {(int)(buff.stats.gunStats.fireSpeed * 100f)}% fire speed for {buff.duration} seconds. " +
            $"(New Fire Rate: {stats.gunStats.fireSpeed * (1f + buff.stats.gunStats.fireSpeed)}).";
    }

    public override void OnFire(Player player, Gun gun)
    {
        if (IsValidTrigger())
        {
            base.OnFire(player, gun);
            player.AddBuff(buff.GetInstance());
        }
    }
}
