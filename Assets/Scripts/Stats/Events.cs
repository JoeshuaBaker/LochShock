using System.Collections.Generic;
[System.Serializable]
public class Events
{
    public List<OnFireAction> OnFire = new List<OnFireAction>();
    public List<OnHitAction> OnHit = new List<OnHitAction>();
    public List<OnKillAction> OnKill = new List<OnKillAction>();
    public List<OnReloadAction> OnReload = new List<OnReloadAction>();
    public List<OnSecondAction> OnSecond = new List<OnSecondAction>();

    public Events()
    {
        OnFire = new List<OnFireAction>();
        OnHit = new List<OnHitAction>();
        OnKill = new List<OnKillAction>();
        OnReload = new List<OnReloadAction>();
        OnSecond = new List<OnSecondAction>();
    }

    public Events Copy()
    {
        Events copy = new Events();
        copy.OnFire.AddRange(this.OnFire);
        copy.OnHit.AddRange(this.OnHit);
        copy.OnKill.AddRange(this.OnKill);
        copy.OnReload.AddRange(this.OnReload);
        copy.OnSecond.AddRange(this.OnSecond);

        return copy;
    }
}
