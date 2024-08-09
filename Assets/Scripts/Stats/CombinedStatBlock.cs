using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CombinedStatBlock
{
    private class OrderedStats
    {
        private Dictionary<Type, SortedList<StatCombineType, HashSet<Stat>>> orderedStats;

        public OrderedStats()
        {
            orderedStats = new Dictionary<Type, SortedList<StatCombineType, HashSet<Stat>>>();
        }

        public void AddStat(Stat stat)
        {
            Type statType = stat.GetType();
            StatCombineType combineType = stat.combineType;

            if (!orderedStats.ContainsKey(stat.GetType()))
            {
                orderedStats.Add(statType, new SortedList<StatCombineType, HashSet<Stat>>());
            }

            SortedList<StatCombineType, HashSet<Stat>> statsByCombineType = orderedStats[statType];

            if (!statsByCombineType.ContainsKey(combineType))
            {
                statsByCombineType.Add(combineType, new HashSet<Stat>());
            }

            HashSet<Stat> statBucket = statsByCombineType[combineType];

            statBucket.Add(stat);
        }

        //returns bool indicating whether all stat buckets are empty, and residual stat should be cleaned up
        public bool RemoveStat(Stat stat)
        {
            Type statType = stat.GetType();
            StatCombineType combineType = stat.combineType;

            if (orderedStats.ContainsKey(statType) && orderedStats[statType].ContainsKey(combineType))
            {
                HashSet<Stat> statBucket = orderedStats[statType][combineType];
                statBucket.Remove(stat);

                //check if all buckets for a stat are 0
                foreach (var bucket in orderedStats[statType].Values)
                {
                    if (bucket.Count > 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public float CalculateStat(Type statType, GameContext context)
        {
            if (!statType.IsSubclassOf(typeof(Stat)) || orderedStats == null || !orderedStats.ContainsKey(statType))
                return 0;

            SortedList<StatCombineType, HashSet<Stat>> sortedStats = orderedStats[statType];
            if (sortedStats == null)
                return 0;

            float baseValue = 0;
            float aggregate = 0;

            foreach (HashSet<Stat> statBucket in sortedStats.Values)
            {
                if (statBucket.Count != 0)
                {
                    Stat first = statBucket.First();

                    foreach (Stat stat in statBucket)
                    {
                        stat.CheckSetConditionStacks(context);
                    }

                    first.combineType.Combine(ref baseValue, ref aggregate, statBucket);
                }
            }

            return aggregate != 0 ? aggregate : baseValue;
        }

        public float CalculateStat<T>(GameContext context) where T : Stat
        {
            return CalculateStat(typeof(T), context);
        }
    }

    public StatBlock combinedStatBlock;
#if UNITY_EDITOR
    //Debug serializable list of sources to cross-check the combined stat block
    public List<StatBlock> sourcesList;
#endif
    private HashSet<StatBlock> sources;
    private OrderedStats orderedStats;

    public CombinedStatBlock()
    {
        sources = new HashSet<StatBlock>();
        orderedStats = new OrderedStats();
        combinedStatBlock = new StatBlock();
    }

    private void AddSource(StatBlock block)
    {
        sources.Add(block);

        foreach(Stat stat in block.stats)
        {
            orderedStats.AddStat(stat);
        }

        combinedStatBlock.Add(block);
    }

    private void CleanupSource(StatBlock block)
    {
        foreach (Stat stat in block.stats)
        {
            if(orderedStats.RemoveStat(stat))
            {
                Stat removeStat = combinedStatBlock.GetStat(stat.GetType());
                combinedStatBlock.stats.Remove(removeStat);
            }
        }

        combinedStatBlock.Remove(block);
    }

    public void UpdateSources(IEnumerable<StatBlock> statBlocks)
    {
        UpkeepSources(statBlocks);
    }

    public float GetCombinedStatValue<T>(GameContext context) where T : Stat
    {
        return orderedStats.CalculateStat<T>(context);
    }

    public float GetCombinedStatValue(Type statType, GameContext context)
    {
        return orderedStats.CalculateStat(statType, context);
    }

    private void UpkeepSources(IEnumerable<StatBlock> statBlocks)
    {
        //Mark all sources potentially inactive this frame
        foreach (var source in sources)
        {
            source.active = false;
        }

        //For each block passed on this frame, mark them active, and add them to the sources list if not already there
        foreach (var block in statBlocks)
        {
            if (!sources.Contains(block))
            {
                AddSource(block);
            }

            block.active = true;
        }

        //Cleanup any sources not provided this frame
        foreach (var source in sources)
        {
            if (!source.active)
            {
                CleanupSource(source);
            }
        }

        sources.RemoveWhere(x => !x.active);

#if UNITY_EDITOR
        sourcesList = sources.ToList();
#endif
    }

    public StatBlockContext GetCombinedContext()
    {
        StatBlockContext statBlockContext = new StatBlockContext();

        foreach (StatBlock statBlock in sources)
        {
            statBlock.UpdateStatBlockContext(ref statBlockContext);
        }

        return statBlockContext;
    }
}
