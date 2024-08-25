using UnityEngine;

[CreateAssetMenu(fileName = "OnSecondApplyBuff", menuName = "NemesisShock/Events/OnSecond/ApplyBuff")]
public class OnSecondApplyBuff : OnSecondAction
{
    public Buff buff;
    public override string GetTooltip(CombinedStatBlock stats, int level = 1)
    {
        return GetBuffTooltip(this, buff, level);
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
