using BulletHell;
using UnityEngine;

[CreateAssetMenu(fileName = "OnKillApplyBuff", menuName = "NemesisShock/Events/OnKill/ApplyBuff")]
public class OnKillApplyBuff : OnKillAction
{
    public Buff buff;

    public override string GetTooltip(StatBlock stats)
    {
        return $"{(int)(chanceToTrigger * 100f)}% chance to gain {(int)(buff.stats.gunStats.fireSpeed * 100f)}% fire speed for {buff.duration} seconds. " +
            $"(New Fire Rate: {stats.gunStats.fireSpeed * (1f + buff.stats.gunStats.fireSpeed)}).";
    }

    public override void OnKill(Player player, Gun gun, ProjectileData projectile, Enemy enemy)
    {
        if (IsValidTrigger())
        {
            base.OnKill(player, gun, projectile, enemy);
            player.AddBuff(buff.GetInstance());
        }
    }
}
