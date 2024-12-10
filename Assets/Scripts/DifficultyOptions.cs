using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelLoader", menuName = "NemesisShock/DifficultyOptions")]
public class DifficultyOptions : ScriptableObject, ILevelLoadComponent
{
    [Flags]
    public enum AscensionModifiers
    {
        DeathWallOrbSpeed = 1,
        FrequentRifts = 2,
        PowerfulRifts = 4,
        FrequentOrbs = 8,
        FastButFrail = 16,
        TrueFinalBoss = 32,
        PowerfulBosses = 64,
        PowerfulEmitters = 128,
        PowerfulBeginning = 256
    }

    public AscensionModifiers ascensionModifiers;

    //Loadable Interface Functions
    public string LoadLabel()
    {
        return "Unspeakable Terrors";
    }

    public int LoadPriority()
    {
        return 900;
    }

    public void Load(World world)
    {
        world.settings.difficultyOptions = this;
    }
}
