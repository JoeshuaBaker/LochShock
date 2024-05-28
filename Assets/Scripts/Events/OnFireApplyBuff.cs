using UnityEngine;

[CreateAssetMenu(fileName = "OnFireApplyBuff", menuName = "NemesisShock/Events/OnFire/ApplyBuff")]
public class OnFireApplyBuff : OnFireAction
{
    public Buff buff;
    public override string GetTooltip(StatBlock stats)
    {
        return GetBuffTooltip(this, buff);
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
