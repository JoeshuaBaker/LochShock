public abstract class OnFireAction : ScriptableAction
{
    public virtual void OnFire(Player player, Gun gun)
    {
        PlayTriggerSfx(player.gameObject);
    }

    public override string GetLabel()
    {
        return "On Fire";
    }
}
