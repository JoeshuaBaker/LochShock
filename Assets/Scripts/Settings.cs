using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "NemesisShock/Settings")]
public class Settings : ScriptableObject, ILevelLoadComponent
{
    public DifficultyOptions difficultyOptions;

    public void Load(World world)
    {
        world.level.settings = this;
    }

    public string LoadLabel()
    {
        return "Settings";
    }

    public int LoadPriority()
    {
        return 1;
    }
}
