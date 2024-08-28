using UnityEngine;

public abstract class OnSecondAction : ScriptableAction
{
    public virtual void OnSecond(Item source, Player player)
    {
        PlayTriggerSfx(player.gameObject);
    }

    public override string GetLabel()
    {
        float actualCooldown = 1f;
        if(cooldown > 0)
        {
            actualCooldown = cooldown;
        }
        return $"Every {actualCooldown} second{((actualCooldown > 1) ? "s":"")}:";
    }
}
