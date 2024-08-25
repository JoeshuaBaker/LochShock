public abstract class OnReloadAction : ScriptableAction
{
    public virtual void OnReload(Item source, Player player, Gun gun)
    {
        PlayTriggerSfx(player.gameObject);
    }

    public override string GetLabel()
    {
        return "On Full Reload";
    }
}
