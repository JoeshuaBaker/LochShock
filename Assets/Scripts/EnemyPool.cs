using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
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
        foreach (EnemyBuffer buffer in enemyBuffers)
        {
            Enemy[] enemyArray = new Enemy[buffer.bufferSize];
            Enemy enemyPrefab = buffer.enemyPrefab;

            GameObject bufferParent = new GameObject();
            bufferParent.transform.parent = enemyParent;
            bufferParent.name = enemyPrefab.name + "s";

            enemies.Add(enemyPrefab.GetType(), enemyArray);
            activeEnemyTypes.Add(enemyPrefab.GetType());
            for (int i = 0; i < buffer.bufferSize; i++)
            {
                enemyArray[i] = Instantiate(buffer.enemyPrefab, bufferParent.transform);
                enemyArray[i].gameObject.SetActive(false);
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
                    continue;
                }
            }

            if(instance != null)
            {
                instance.Reset();
                instance.instanceId = i;
            }
        }

        return instance;
    }

    public List<Type> GetAvailableEnemyTypes()
    {
        return activeEnemyTypes;
    }
}
