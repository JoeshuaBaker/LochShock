using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteCoordinator : MonoBehaviour
{
    public EliteEnemy elitePrefab;
    protected List<EliteEnemy> activeEnemies;
    protected List<EliteEnemy> deactivatedEnemies;
    protected List<EliteEnemy> cleanupList;
    public int activeLimit = 25;

    private void Start()
    {
        activeEnemies = new List<EliteEnemy>();
        deactivatedEnemies = new List<EliteEnemy>();
        cleanupList = new List<EliteEnemy>();

        //Test spawn a dummy elite, typically we should go through EnemyPool.GetEnemy (elite type)
        SpawnElite();
    }

    public EliteEnemy SpawnElite()
    {
        int currentActiveEnemies = activeEnemies.Count;
        if (currentActiveEnemies >= activeLimit)
        {
            return null;
        }

        EliteEnemy elite;

        if (deactivatedEnemies.Count > 0)
        {
            elite = deactivatedEnemies.First();
            deactivatedEnemies.Remove(elite);
        }
        else
        {
            elite = Instantiate(elitePrefab, this.transform);
        }

        activeEnemies.Add(elite);
        elite.Setup(this);
        return elite;
    }

    public List<EliteEnemy> SpawnElites(int count)
    {
        int currentActiveEnemies = activeEnemies.Count;
        int numElitesToSpawn = Mathf.Min(count, activeLimit - currentActiveEnemies);
        List<EliteEnemy> newElites = new List<EliteEnemy>();

        if(numElitesToSpawn > 0)
        {
            for(int i = 0; i < numElitesToSpawn; i++)
            {
                EliteEnemy elite = SpawnElite();
                if(elite != null)
                {
                    newElites.Add(elite);
                }
            }
        }

        return newElites;
    }

    public virtual void UpdateActiveEnemies()
    {
        foreach(EliteEnemy elite in activeEnemies)
        {
            elite.CoordinatorUpdate();
        }

        CleanupActiveEnemies();
    }

    public virtual void CleanupActiveEnemies()
    {
        //Get list of enemies that should be cleaned up
        cleanupList.AddRange(activeEnemies.Where(x => !x.IsAlive));
        cleanupList.ForEach(x => x.Cleanup());

        //Update other lists
        activeEnemies.RemoveRange(cleanupList);
        deactivatedEnemies.AddRange(cleanupList);
        cleanupList.Clear();
    }

    public Type EliteType()
    {
        if (elitePrefab != null)
        {
            return elitePrefab.GetType();
        }
        else return null;
    }
}
