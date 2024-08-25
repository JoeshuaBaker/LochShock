using BulletHell;
using UnityEngine;

[CreateAssetMenu(fileName = "OnHitApplyBuff", menuName = "NemesisShock/Events/OnHit/ApplyBuff")]
public class OnHitApplyBuff : OnHitAction
{
    public Buff buff;

    public override string GetTooltip(CombinedStatBlock stats, int level = 1)
    {
        return GetBuffTooltip(this, buff, level);
    }

    public override void OnHit(Item source, Player player, Gun gun, ProjectileData projectile, Enemy enemy)
    {
        if(IsValidTrigger())
        {
            base.OnHit(source, player, gun, projectile, enemy);
            source.AddBuff(buff.GetInstance(source));
        }
    }
}
