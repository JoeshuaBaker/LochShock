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
    public List<EliteCoordinator> eliteBuffers;
    public Dictionary<Type, Enemy[]> enemies;
    private List<Type> activeEnemyTypes;
    public Transform enemyParent;

    public List<EnemyBlock> presetBlocks;
    public List<EnemyBlock> randomBlocks;
    private Queue<int> randomIndexGrabBag;
    [SerializeField] private EnemyBlock currentBlock;
    private int presetIndex;
    private int randomIndex;
    [SerializeField] private float blockTimer;

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
        presetIndex = 0;
        randomIndex = 0;
        blockTimer = 0f;

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

        SetNewEnemyBlock();

        List<EliteCoordinator> eliteCoordinatorPrefabs = eliteBuffers;
        eliteBuffers = new List<EliteCoordinator>();
        foreach(EliteCoordinator buffer in eliteCoordinatorPrefabs)
        {
            eliteBuffers.Add(Instantiate(buffer, enemyParent));
        }
    }

    public Enemy GetEnemy(Type enemyType)
    {
        Enemy instance = null;

        if(enemyType.IsSubclassOf(typeof(EliteEnemy)))
        {
            foreach(EliteCoordinator coordinator in eliteBuffers)
            {
                if(enemyType == coordinator.EliteType())
                {
                    instance = coordinator.SpawnElite();
                    break;
                }
            }
        }

        else if (enemies.ContainsKey(enemyType))
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

    public Enemy GetCurrentBlockEnemy()
    {
        Type enemyType = null;

        float totalRatio = currentBlock.enemyBlocks.Select(x => x.spawnRate).Sum();
        float randWithinRatio = UnityEngine.Random.Range(0, totalRatio);
        float ratioSelector = 0f;

        foreach (var entry in currentBlock.enemyBlocks)
        {
            ratioSelector += entry.spawnRate;
            if (randWithinRatio < ratioSelector)
            {
                enemyType = entry.GetEnemyType();
                break;
            }
        }

        if(enemyType != null)
        {
            return GetEnemy(enemyType);
        }

        return null;
    }

    public List<Type> GetAvailableEnemyTypes()
    {
        return activeEnemyTypes;
    }

    private void Update()
    {
        blockTimer = Mathf.Max(blockTimer - Time.deltaTime, 0f);
        if(blockTimer <= 0f)
        {
            SetNewEnemyBlock();
        }

        if(!World.activeWorld.paused)
        {
            foreach (EliteCoordinator eliteCoordinator in eliteBuffers)
            {
                eliteCoordinator.UpdateActiveEnemies();
            }
        }
    }

    private void SetNewEnemyBlock()
    {
        if(presetIndex < presetBlocks.Count)
        {
            currentBlock = presetBlocks[presetIndex];
            presetIndex++;
            blockTimer = currentBlock.duration;
        }
        else
        {
            if(randomIndexGrabBag == null || randomIndexGrabBag.Count() == 0)
            {
                randomIndexGrabBag = new Queue<int>(Enumerable.Range(0, randomBlocks.Count()).Shuffle());
            }

            randomIndex = randomIndexGrabBag.Dequeue();
            currentBlock = randomBlocks[randomIndex];
            blockTimer = currentBlock.duration;
        }
    }
}
