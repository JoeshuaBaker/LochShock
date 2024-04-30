using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public List<EnemyBuffer> enemyBuffers;
    public Dictionary<Type, Enemy[]> enemies;
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
        foreach (EnemyBuffer buffer in enemyBuffers)
        {
            Enemy[] enemyArray = new Enemy[buffer.bufferSize];
            Enemy enemyPrefab = buffer.enemyPrefab;

            GameObject bufferParent = new GameObject();
            bufferParent.transform.parent = enemyParent;
            bufferParent.name = enemyPrefab.name + "s";

            enemies.Add(enemyPrefab.GetType(), enemyArray);
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
}
