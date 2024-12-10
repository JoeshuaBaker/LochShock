using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelLoader", menuName = "NemesisShock/LevelLoader")]
public class LevelLoader : ScriptableObject
{
    //ILevelLoadComponents
    public Level level;
    // Settings / Options in general

    //state variables
    public string loadingString;

    public List<ILevelLoadComponent> GetLoadableComponents()
    {
        return level.GetLoadableComponents();
    }

    public void UpdateLoadingString(string label)
    {
        loadingString = "Now Loading " + label + "...";
    }

    public void UpdateLoadingString(ILevelLoadComponent loadableComponent)
    {
        UpdateLoadingString(loadableComponent.LoadLabel());
    }
}
