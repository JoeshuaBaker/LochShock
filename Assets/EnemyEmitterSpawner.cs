using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell;

public class EnemyEmitterSpawner : MonoBehaviour
{
    public Transform enemyEmitterParent;
    public EnemyGun[] enemyEmitters;
    public AnimationCurve emitterSpawnFrequencyCurve;
    public AnimationCurve emitterPowerBudgetCurve;
    [Range(0,1)] public float enemyDifficultyFactor = 0.25f;
    [Range(0,1)] public float orbDifficultyFactor = 0.75f;
    public float emitterSpawnCooldownMax = 60f;
    public float emitterSpawnCooldownMin = 3f;
    public float powerBudgetFloor = 0;
    public float powerBudgetCeil = 1000f;
    public int onlySpawnIfThereAreNChoices = 1;

    //internal state variables
    [Header("Internal State/Debug Variables")]
    private SortedList<float, EnemyGun> emittersByPower;
    [SerializeField] private float difficultySelector = 0f;
    [SerializeField] private float percentReadyToSpawn = 0f;
    [SerializeField] private float budgetUsed = 0f;
    [SerializeField] private float currentBudget = 0f;
    [SerializeField] private List<EnemyGun> activeEmitters;
    [SerializeField] private float currentSpawnCooldown = 0f;
    [SerializeField] private float lowestPower;

    // Start is called before the first frame update
    void Start()
    {
        emittersByPower = new SortedList<float, EnemyGun>();
        lowestPower = float.MaxValue;
        activeEmitters = new List<EnemyGun>();
        for(int i = 0; i < enemyEmitters.Length; i++)
        {
            EnemyGun enemyGun = Instantiate(enemyEmitters[i], enemyEmitterParent);
            enemyGun.shooting = false;

            float powerRating = enemyGun.PowerRating;
            if (powerRating < lowestPower)
                lowestPower = powerRating;
            while (emittersByPower.ContainsKey(powerRating))
                powerRating += Random.Range(-1f, 1f);

            emittersByPower.Add(powerRating, enemyGun);
        }
    }

    public void UpdateEmitterSpawner(float enemyDifficulty, float orbDifficulty)
    {
        difficultySelector = enemyDifficulty * enemyDifficultyFactor + orbDifficulty * orbDifficultyFactor;
        float spawnFrequency = emitterSpawnFrequencyCurve.Evaluate(difficultySelector);
        if (spawnFrequency > 0f)
            currentSpawnCooldown = Mathf.Lerp(emitterSpawnCooldownMax, emitterSpawnCooldownMin, spawnFrequency);
        else
            currentSpawnCooldown = 0;

        activeEmitters.RemoveAll(x => x.ready);

        if(currentSpawnCooldown > 0)
        {
            percentReadyToSpawn = Mathf.Min(percentReadyToSpawn + (Time.deltaTime / currentSpawnCooldown), 1f);
        }

        currentBudget = Mathf.Lerp(powerBudgetFloor, powerBudgetCeil, emitterPowerBudgetCurve.Evaluate(difficultySelector));

        budgetUsed = 0f;
        foreach(EnemyGun emitter in activeEmitters)
        {
            budgetUsed += emitter.PowerRating;
            emitter.transform.position = Player.activePlayer.transform.position + emitter.screenPositionOffset;
        }
        float availableBudget = currentBudget - budgetUsed;

        if (spawnFrequency > 0f && percentReadyToSpawn == 1f && availableBudget > lowestPower)
        {
            Debug.Log(emittersByPower.Count);
            IList<float> keys = emittersByPower.Keys;
            int highestPowerAvailableIndex = 0;
            for(int i = 0; i < keys.Count; i++)
            {
                if(keys[i] > currentBudget || i == keys.Count - 1)
                {
                    highestPowerAvailableIndex = i;
                    break;
                }
            }

            if(highestPowerAvailableIndex < onlySpawnIfThereAreNChoices)
            {
                Debug.Log("returning because there are only: " + highestPowerAvailableIndex + " choices.");
                return;
            }

            int emitterIndexToSpawn = Random.Range(0, highestPowerAvailableIndex);
            float emitterToSpawnKey = keys[emitterIndexToSpawn];
            EnemyGun emitter = emittersByPower[emitterToSpawnKey];
            if(!emitter.ready)
            {
                Debug.Log("returning because the emitter we tried to spawn is already active.");
                return;
            }

            int XYSelector = Random.Range(0, 2);
            float x = 0;
            float y = 0;
            if(XYSelector == 0)
            {
                x = 1f;
                y = Random.Range(0f, 1f);
            }
            else
            {
                y = Mathf.Round(Random.Range(0f, 1f));
                x = Random.Range(0.5f, 1f);
            }
            Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(x*Camera.main.pixelWidth, y*Camera.main.pixelHeight, -Camera.main.transform.position.z));
            emitter.Setup(position);
            activeEmitters.Add(emitter);
            percentReadyToSpawn = 0f;
        }
    }
}
