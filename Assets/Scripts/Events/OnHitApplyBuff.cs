using BulletHell;
using UnityEngine;

[CreateAssetMenu(fileName = "OnHitApplyBuff", menuName = "NemesisShock/Events/OnHit/ApplyBuff")]
public class OnHitApplyBuff : OnHitAction
{
    public Buff buff;

    public override string GetTooltip(CombinedStatBlock stats)
    {
        return GetBuffTooltip(this, buff);
    }

    public override void OnHit(Player player, Gun gun, ProjectileData projectile, Enemy enemy)
    {
        if(IsValidTrigger())
        {
            base.OnHit(player, gun, projectile, enemy);
            player.AddBuff(buff.GetInstance(source));
        }
    }
}
