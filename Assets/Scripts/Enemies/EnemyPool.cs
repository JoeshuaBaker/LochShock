using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour, ILevelLoadComponent
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

    public float horizontalSpawnBarrier = 16f;
    public float verticalSpawnBarrier = 12f;
    public float enemySpawn;
    public float enemySpawnRate = 1f;
    public float adjustedSpawnRate = 1f;
    public float enemySpawnRateFloor = .01f;
    public AnimationCurve enemySpawnRateDecay = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve orbDecay = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float enemySpawnRateDecayTime = 10f;
    public float orbDecayMaxtime = 1.5f;

    private bool ready = false;

    [Serializable]
    public class EnemyBuffer
    {
        public Enemy enemyPrefab;
        public int bufferSize;
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
        if(ready)
        {
            blockTimer = Mathf.Max(blockTimer - Time.deltaTime, 0f);
            if (blockTimer <= 0f)
            {
                SetNewEnemyBlock();
            }

            if (!World.activeWorld.paused)
            {
                foreach (EliteCoordinator eliteCoordinator in eliteBuffers)
                {
                    eliteCoordinator.UpdateActiveEnemies();
                }
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


    //Loadable Interface Functions
    public string LoadLabel()
    {
        return "Enemies";
    }

    public int LoadPriority()
    {
        return 1000;
    }

    public void Setup()
    {
        
    }

    public void Load(World world)
    {
        EnemyPool enemyPoolInstance = Instantiate(this, world.transform);
        enemyPoolInstance.SetupEnemies();
        world.level.enemyPool = enemyPoolInstance;
    }

    public void SetupEnemies()
    {
        enemySpawn = 0f;
        ready = true;

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
        foreach (EliteCoordinator buffer in eliteCoordinatorPrefabs)
        {
            eliteBuffers.Add(Instantiate(buffer, enemyParent));
        }
    }

    void SetupEnemy(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        bool leftright = UnityEngine.Random.Range(0f, 1f) < 0.5f;
        bool updown = UnityEngine.Random.Range(0f, 1f) < 0.5f;
        Vector2 diagonal = new Vector2(leftright ? 1 : -0.25f, updown ? 1 : -1);
        Vector2 lerpVecBase = Vector2.right;

        if (!leftright)
        {
            if (updown)
            {
                lerpVecBase = new Vector2(1, 1);
            }
            else
            {
                lerpVecBase = new Vector2(1, -1);
            }
        }
        Vector2 lerpVec = Vector2.Lerp(lerpVecBase, diagonal, UnityEngine.Random.Range(0f, 1f));
        Vector3 spawnPosition = new Vector3(
            horizontalSpawnBarrier * lerpVec.x, verticalSpawnBarrier * lerpVec.y, 0) + (Player.activePlayer.transform.position - this.transform.position);

        enemy.transform.localPosition = spawnPosition;
        enemy.Reset();
    }

    void SpawnRandomEnemy()
    {
        List<Type> enemyTypes = GetAvailableEnemyTypes();
        if (enemyTypes == null || enemyTypes.Count == 0)
        {
            return;
        }

        Enemy enemy = GetEnemy(enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count)]);
        SetupEnemy(enemy);
    }

    void SpawnRandomBlockEnemy()
    {
        Enemy enemy = GetCurrentBlockEnemy();
        SetupEnemy(enemy);
    }

    public void UpdateEnemies()
    {
        enemySpawn -= Time.deltaTime;
        float levelLoadTimeRatio = Mathf.Min(Time.timeSinceLevelLoad / (enemySpawnRateDecayTime * 60f), 1f);
        float orbTimeRatio = Mathf.Min(Player.activePlayer.timeSinceOrbUsed / (orbDecayMaxtime * 60f), 1f);
        adjustedSpawnRate = Mathf.Lerp(enemySpawnRate, enemySpawnRateFloor, enemySpawnRateDecay.Evaluate(
            Mathf.Min(levelLoadTimeRatio + (1f - levelLoadTimeRatio) * orbDecay.Evaluate(orbTimeRatio), 1f)));

        while (enemySpawn < adjustedSpawnRate)
        {
            enemySpawn += adjustedSpawnRate;
            SpawnRandomBlockEnemy();
        }

        World.activeWorld.worldStaticContext.activeEnemies = activeEnemies;

        Enemy closestEnemy = null;
        float distanceSqr = float.MaxValue;
        Vector3 playerPosition = Player.activePlayer.transform.position;

        var worldStaticContext = World.activeWorld.worldStaticContext;
        foreach (Enemy enemy in worldStaticContext.activeEnemies)
        {
            float newDist = (enemy.transform.position - playerPosition).sqrMagnitude;
            if (newDist < distanceSqr)
            {
                closestEnemy = enemy;
                distanceSqr = newDist;
            }
        }
        worldStaticContext.closestEnemy = closestEnemy;

        World.activeWorld.UpdateEnemyEmitters(enemySpawnRateDecay.Evaluate(levelLoadTimeRatio), orbDecay.Evaluate(orbTimeRatio));
    }
}
