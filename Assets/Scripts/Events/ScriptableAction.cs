using UnityEngine;

[System.Serializable]
public abstract class ScriptableAction : ScriptableObject
{
    public float chanceToTrigger = 1.0f;
    public float cooldown = 0f;
    private double lastTriggerTime = -1;
    public abstract string GetLabel();
    public abstract string GetTooltip(StatBlock stats);

    public bool IsValidTrigger()
    {
        bool isValidTrigger = 
            Random.Range(0f, 1f) <= chanceToTrigger && 
            (cooldown == 0 || lastTriggerTime == -1 || Time.timeAsDouble > lastTriggerTime + cooldown);
        if(isValidTrigger)
        {
            lastTriggerTime = Time.timeAsDouble;
        }

        return isValidTrigger;
    }
}
