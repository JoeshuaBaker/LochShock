public abstract class OnReloadAction : ScriptableAction
{
    public virtual void OnReload(Player player, Gun gun)
    {
        PlayTriggerSfx(player.gameObject);
    }

    public override string GetLabel()
    {
        return "On Reload";
    }
}
