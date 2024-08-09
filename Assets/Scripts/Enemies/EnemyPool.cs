using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public Enemy[] allEnemies;
    public HashSet<Enemy> activeEnemies;
    public List<EnemyBuffer> enemyBuffers;
    public Dictionary<Type, Enemy[]> enemies;
    private List<Type> activeEnemyTypes;
    public Transform enemyParent;

    [Serializable]
    public class EnemyBuffer
    {
        public Enemy enemyPrefab;
        public int bufferSize;
    }

    void Start()
    {
        enemies = new Dictionary<Type, Enemy[]>();
        activeEnemyTypes = new List<Type>();
        int allEnemiesBufferSize = enemyBuffers.Select(x => x.bufferSize).Sum();
        allEnemies = new Enemy[allEnemiesBufferSize];
        activeEnemies = new HashSet<Enemy>();

        foreach (EnemyBuffer buffer in enemyBuffers)
        {
            Enemy[] enemyArray = new Enemy[buffer.bufferSize];
            allEnemiesBufferSize += buffer.bufferSize;
            Enemy enemyPrefab = buffer.enemyPrefab;

            GameObject bufferParent = new GameObject();
            bufferParent.transform.parent = enemyParent;
            bufferParent.name = enemyPrefab.name + "s";

            enemies.Add(enemyPrefab.GetType(), enemyArray);
            activeEnemyTypes.Add(enemyPrefab.GetType());
            int acc = 0;
            for (int i = 0; i < buffer.bufferSize; i++)
            {
                enemyArray[i] = Instantiate(buffer.enemyPrefab, bufferParent.transform);
                enemyArray[i].name = enemyPrefab.GetType().ToString() + " " + i;
                enemyArray[i].gameObject.SetActive(false);
                allEnemies[acc++] = enemyArray[i];
            }
        }
    }

    public Enemy GetEnemy(Type enemyType)
    {
        Enemy instance = null;

        if (enemies.ContainsKey(enemyType))
        {
            Enemy[] enemyArray = enemies[enemyType];
            int i;
            for(i = 0; i < enemyArray.Length; i++)
            {
                if(!enemyArray[i].gameObject.activeSelf)
                {
                    instance = enemyArray[i];
                    break;
                }
            }

            if(instance != null)
            {
                instance.instanceId = i;
                activeEnemies.Add(instance);
            }
        }

        return instance;
    }

    public List<Type> GetAvailableEnemyTypes()
    {
        return activeEnemyTypes;
    }
}
