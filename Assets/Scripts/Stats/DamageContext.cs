using System.Collections.Generic;

public struct DamageContext
{
    public int bulletInMag;
    public int maxBulletsInMag;
    public int numBounces;
    public int numPierces;
    public DamageType damageType;
    public Item source;
    public HashSet<Enemy> hitEnemies;
    public BossSeed hitBoss;
}