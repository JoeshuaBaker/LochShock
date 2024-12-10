using System.Collections.Generic;

[System.Serializable]
public class Level
{
    public EnemyPool enemyPool;
    public MapPool mapPool;
    public EnemyEmitterSpawner enemyEmitterSpawner;
    public DoodadManager doodadManager;
    public BossSeed boss;
    public EnvironmentalEffectController environmentalEffects;
    public Settings settings;

    public List<ILevelLoadComponent> GetLoadableComponents()
    {
        List<ILevelLoadComponent> loadableComponents = new List<ILevelLoadComponent>() { 
            enemyPool, 
            mapPool, 
            enemyEmitterSpawner, 
            doodadManager, 
            boss, 
            environmentalEffects, 
            settings 
        };
        loadableComponents.Sort((x, y) => x.LoadPriority().CompareTo(y.LoadPriority()));

        return loadableComponents;
    }
}
