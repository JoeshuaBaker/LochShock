using System.Collections.Generic;

public struct GameContext
{
    public Player player;
    public IEnumerable<Enemy> activeEnemies;
    public Enemy closestEnemy;
    public DamageContext damageContext;
}
