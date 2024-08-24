using UnityEngine;

[CreateAssetMenu(fileName = "OnSecondApplyBuff", menuName = "NemesisShock/Events/OnSecond/ApplyBuff")]
public class OnSecondApplyBuff : OnSecondAction
{
    public Buff buff;
    public override string GetTooltip(CombinedStatBlock stats)
    {
        return GetBuffTooltip(this, buff);
    }

    public override void OnSecond(Item source, Player player)
    {
        if (IsValidTrigger())
        {
            base.OnSecond(source, player);
            source.AddBuff(buff.GetInstance(source));
        }
    }
}
