using BulletHell;

public abstract class OnKillAction : ScriptableAction
{
    public virtual void OnKill(Player player, Gun gun, ProjectileData projectile, Enemy enemy)
    {
        PlayTriggerSfx(player.gameObject);
    }

    public override string GetLabel()
    {
        return "On Kill";
    }
}
