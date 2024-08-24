using UnityEngine;

[CreateAssetMenu(fileName = "OnFireApplyBuff", menuName = "NemesisShock/Events/OnFire/ApplyBuff")]
public class OnFireApplyBuff : OnFireAction
{
    public Buff buff;
    public override string GetTooltip(CombinedStatBlock stats)
    {
        return GetBuffTooltip(this, buff);
    }

    public override void OnFire(Item source, Player player, Gun gun)
    {
        if (IsValidTrigger())
        {
            base.OnFire(source, player, gun);
            source.AddBuff(buff.GetInstance(source));
        }
    }
}
