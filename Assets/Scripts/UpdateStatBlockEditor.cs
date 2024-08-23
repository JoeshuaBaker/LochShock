using UnityEngine;

[RequireComponent(typeof(Item))]
public class UpdateStatBlockEditor : MonoBehaviour
{
    [InspectorButton("CopyStats", ButtonWidth = 240f)]
    public bool copyStats = false;

    [InspectorButton("RecopyStats", ButtonWidth = 240f)]
    public bool recopyStats = false;

    public StatBlock copiedStats;
    public StatBlock levelUpStats;

    public void CopyStats()
    {
        Item item = GetComponent<Item>();
        copiedStats = item.stats.DeepCopy();
        levelUpStats = item.levelUpStats.DeepCopy();
    }

    public void RecopyStats()
    {
        Item item = GetComponent<Item>();
        foreach (Stat stat in copiedStats.stats)
        {
            Stat itemStat;
            if ((itemStat = item.stats.GetStat(stat.GetType())) != null)
            {
                if (stat is FireSpeed)
                {
                    itemStat.value = 1f / stat.value;
                }
                else
                {
                    itemStat.value = stat.value;
                }
            }
            else
            {
                item.stats.stats.Add(stat);
            }
        }

        foreach (Stat stat in levelUpStats.stats)
        {
            Stat itemStat;
            if ((itemStat = item.levelUpStats.GetStat(stat.GetType())) != null)
            {
                if (stat is FireSpeed)
                {
                    itemStat.value = 1f / stat.value;
                }
                else
                {
                    itemStat.value = stat.value;
                }
            }
            else
            {
                item.levelUpStats.stats.Add(stat);
            }
        }
    }
}
