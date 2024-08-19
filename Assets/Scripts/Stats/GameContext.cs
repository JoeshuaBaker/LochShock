using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameContext
{
    public Player player;
    public IEnumerable<Enemy> activeEnemies;
    public Enemy closestEnemy;
    public List<Enemy> hitEnemies;
    public List<BossSeed> hitBoss;
}
