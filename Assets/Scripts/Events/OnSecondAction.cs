using UnityEngine;

public abstract class OnSecondAction : ScriptableAction
{
    public virtual void OnSecond(Player player)
    {
        PlayTriggerSfx(player.gameObject);
    }

    public override string GetLabel()
    {
        return "Periodically";
    }
}
