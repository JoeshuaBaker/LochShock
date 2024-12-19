using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Player player;
    public bool loadLevelFromInspectorOnStart = true;
    public LevelLoader levelLoader;
    public Level level;
    public GameObject spawnContainer;
    public static World activeWorld;
    public bool paused = false;
    public bool ready = false;
    public ExplosionSpawner explosionSpawner;
    public GameContext worldStaticContext;
    public CraterCreator craterCreator;
    public BulletHitEffect hitEffect;
    public LightningBolt lightningBolt;
    public GameplayUI gameplayUi;
    public GameObject crosshairVis;
    public DeathWall deathWall;
    public OrbSpawner orbSpawner;
    public Transform projectilePoolParent;
    public Settings settings => level.settings;
    public EnemyEmitterSpawner enemyEmitterSpawner => level.enemyEmitterSpawner;

    //Map Variables
    public MapPool mapPool => level.mapPool;

    //Enemy Variables
    public EnemyPool enemyPool => level.enemyPool;

    //internal state variables

    [Header("Stage Length Vars")]
    public float stageMaxLength = 5000f;
    public float stageMinLength = 3000f;
    public float stageCurrentLength;
    public float stageCompletionAsPercent;
    public float currentTime;
    public float minutesBeforeLengthDecay = 12f;
    public float lengthDecayPerSecond = 5f;

    //boss variables
    public BossSeed boss => level.boss;
    public BossSeed spawnedBoss;
    public bool bossSpawned;
    public bool bossDead;
    public bool bossDeathFinished;

    private void Start()
    {
        if(loadLevelFromInspectorOnStart)
        {
            LoadLevel(levelLoader);
        }
    }

    void EarlySetup()
    {
        Application.targetFrameRate = -1;
        player = Player.activePlayer;

        if(gameplayUi == null)
        {
            Debug.Log("world needs gameplayUI reference");
        }
        if(crosshairVis == null)
        {
            Debug.Log("world needs CrosshairVisibility reference, Camera->worldspaceui->crosshaircontainer->crosshairVis");
        }
    }

    void LateSetup()
    {
        if (player != null && spawnContainer != null)
        {
            spawnContainer.transform.position = player.transform.position;
        }

        stageCurrentLength=stageMaxLength;
        ready = true;
    }

    public void LoadLevel(LevelLoader levelLoader)
    {
        EarlySetup();

        level = new Level();

        List<ILevelLoadComponent> loadableComponents = levelLoader.GetLoadableComponents();

        foreach(var loadable in loadableComponents)
        {
            levelLoader.UpdateLoadingString(loadable);
            loadable.Load(this);
        }

        LateSetup();
    }

    void Update()
    {
        if (ready)
        {
            mapPool.UpdateMaps();
            enemyPool.UpdateEnemies();
            UpdateStageLength();
            if(stageCompletionAsPercent >= 1)
            {
                UpdateBoss();
            }
        }
    }

    public void Pause(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        paused = pause;
    }

    public void SetupContext()
    {
        activeWorld = this;
        worldStaticContext = new GameContext
        {
            player = Player.activePlayer,
            activeEnemies = new HashSet<Enemy>()
        };
    }

    public Tile RandomPathTileAtXPosition(float xPos)
    {
        return mapPool.RandomPathTileAtXPosition(xPos);
    }

    public bool IsBoundsOnPath(Bounds bounds, bool solidTilesOnly = true)
    {
        return mapPool.IsBoundsOnPath(bounds, solidTilesOnly);
    }

    public List<Tile> TilesWithinBounds(Bounds bounds, bool solidTilesOnly = false)
    {
        return mapPool.TilesWithinBounds(bounds, solidTilesOnly);
    }

    public Tile ClosestTile(Vector3 pos)
    {
        return mapPool.ClosestTile(pos);
    }

    public float ClosestTileDistance(Vector3 pos)
    {
        return mapPool.ClosestTileDistance(pos);
    }

    public Tile TileUnderPlayer(Vector3 position)
    {
        return mapPool.TileUnderPlayer(position);
    }

    public void ClearAllEnemyBullets()
    {
        if(enemyEmitterSpawner != null && enemyEmitterSpawner.isActiveAndEnabled)
        {
            enemyEmitterSpawner.ClearAllBullets();
        }
    }

    public void UpdateEnemyEmitters(float enemyDifficulty, float orbDifficulty)
    {
        if (enemyEmitterSpawner != null && enemyEmitterSpawner.isActiveAndEnabled)
        {
            enemyEmitterSpawner.UpdateEmitterSpawner(enemyDifficulty, orbDifficulty);
        }
    }

    public void UpdateStageLength()
    {
        currentTime += Time.deltaTime;

        if(currentTime>= minutesBeforeLengthDecay * 60f && !bossSpawned)
        {
            if(stageCurrentLength > stageMinLength)
            {
                stageCurrentLength -= lengthDecayPerSecond * Time.deltaTime;
                stageCurrentLength = Mathf.Max(stageCurrentLength, stageMinLength);

            }
        }
        stageCompletionAsPercent = Mathf.Max(player.transform.position.x / stageCurrentLength, stageCompletionAsPercent);
        gameplayUi.SetBossDistance(stageCurrentLength);
    }

    public void UpdateBoss()
    {
        if(player.transform.position.x >= stageCurrentLength && !bossSpawned)
        {
            spawnedBoss = Instantiate(boss);
            spawnedBoss.SetDistance((int)stageCurrentLength);

            spawnedBoss.bossCurrentHP = spawnedBoss.bossMaxHP;

            bossSpawned = true;
            gameplayUi.bossActive = true;
        }
        if(spawnedBoss != null)
        {
            if(spawnedBoss.bossHPPercent <= 0f)
            {

                bossDead = true;
                gameplayUi.bossDead = true;
                player.bossDead = true;
                deathWall.bossDead = true;

                crosshairVis.SetActive(false);

                bossDeathFinished = spawnedBoss.deathFinished;
                gameplayUi.bossDeathFinished = spawnedBoss.deathFinished;

                if (bossDeathFinished)
                {
                    crosshairVis.SetActive(true);
                }
            }

            gameplayUi.bossHpPercent = spawnedBoss.bossHPPercent;
        }
    }

    //working on breaking up horrible boss update function
    public void BossDead()
    {
        bossDead = true;
        gameplayUi.bossDead = true;
        player.bossDead = true;
        deathWall.bossDead = true;

        crosshairVis.SetActive(false);

        bossDeathFinished = spawnedBoss.deathFinished;
        gameplayUi.bossDeathFinished = spawnedBoss.deathFinished;
    }

    public void BossCutsceneOver()
    {
        crosshairVis.SetActive(true);
    }
}
