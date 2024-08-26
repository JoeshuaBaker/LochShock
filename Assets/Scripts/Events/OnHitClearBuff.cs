using BulletHell;
using UnityEngine;

[CreateAssetMenu(fileName = "OnHitClearBuff", menuName = "NemesisShock/Events/OnHit/ClearBuff")]
public class OnHitClearBuff : OnHitAction
{
    public bool clearAllBuffs = false;
    public Buff buffToClear;
    public int stacks;

    public override string GetTooltip(CombinedStatBlock stats, int level = 1)
    {
        return $"Clear {(clearAllBuffs ? "all of this item's buffs" : buffToClear.buffName)}.";
    }

    public override void OnHit(Item source, Player player, Gun gun, ProjectileData projectile, Enemy enemy)
    {
        base.OnHit(source, player, gun, projectile, enemy);
        
        if(stacks == 0)
        {
            if (clearAllBuffs)
                source.ClearBuffs();
            else
                source.RemoveBuff(buffToClear);
        }
        else
        {
            source.RemoveStacksFromBuff(stacks);
        }

    }
}
