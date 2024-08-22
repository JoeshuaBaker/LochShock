using System.Collections.Generic;

public struct DamageContext
{
    public DamageType damageType;
    public Item source;
    public HashSet<Enemy> hitEnemies;
    public BossSeed hitBoss;
}