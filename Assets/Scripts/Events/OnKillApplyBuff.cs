using BulletHell;
using UnityEngine;

[CreateAssetMenu(fileName = "OnKillApplyBuff", menuName = "NemesisShock/Events/OnKill/ApplyBuff")]
public class OnKillApplyBuff : OnKillAction
{
    public Buff buff;

    public override string GetTooltip(CombinedStatBlock stats, int level = 1)
    {
        return GetBuffTooltip(this, buff, level);
    }

    public override void OnKill(Item source, Player player, Gun gun, ProjectileData projectile, Enemy enemy)
    {
        if (IsValidTrigger())
        {
            base.OnKill(source, player, gun, projectile, enemy);
            source.AddBuff(buff.GetInstance(source));
        }
    }
}
