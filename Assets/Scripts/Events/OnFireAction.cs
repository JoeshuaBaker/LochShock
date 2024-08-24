public abstract class OnFireAction : ScriptableAction
{
    public virtual void OnFire(Item source, Player player, Gun gun)
    {
        PlayTriggerSfx(player.gameObject);
    }

    public override string GetLabel()
    {
        return "On Fire";
    }
}
