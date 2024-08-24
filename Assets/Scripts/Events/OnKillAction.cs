using BulletHell;

public abstract class OnKillAction : ScriptableAction
{
    public virtual void OnKill(Item source, Player player, Gun gun, ProjectileData projectile, Enemy enemy)
    {
        PlayTriggerSfx(player.gameObject);
    }

    public override string GetLabel()
    {
        return "On Kill";
    }
}
