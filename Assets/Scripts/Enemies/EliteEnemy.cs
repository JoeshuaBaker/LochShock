public abstract class EliteEnemy : Enemy
{
    public EliteCoordinator coordinator;
    protected bool alive;

    public virtual void Setup(EliteCoordinator coordinator)
    {
        this.coordinator = coordinator;
        this.gameObject.SetActive(true);
        alive = true;
    }

    public virtual void Cleanup()
    {
        this.gameObject.SetActive(false);
        alive = false;
    }

    public override void BombHit(float bombKillDistanceDelay)
    {
        //Todo: Get hit by player's bomb.
    }

    public override void TouchPlayer()
    {
        //Overwrite default behavior for touching the player so we don't die.
    }

    public abstract void CoordinatorUpdate();
    public virtual bool IsAlive => alive;
}
