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
                foreach(var bucket in orderedStats[statType].Values)
                {
                    if(bucket.Count > 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void CombineStats(StatBlock combinedStatBlock)
        {
            foreach(SortedList<StatCombineType, HashSet<Stat>> sortedStats in orderedStats.Values)
            {
                float baseValue = 0;
                float aggregate = 0;
                Type statType = null;

                foreach(HashSet<Stat> statBucket in sortedStats.Values)
                {
                    if(statBucket.Count != 0)
                    {
                        Stat first = statBucket.First();
                        if (statType == null)
                        {
                            statType = first.GetType();
                        }

                        first.combineType.Combine(ref baseValue, ref aggregate, statBucket);
                    }
                }
                
                if(statType != null && (aggregate != 0 || baseValue != 0))
                {
                    Stat combinedStat = combinedStatBlock.GetStat(statType);
                    if (combinedStat == null)
                    {
                        combinedStat = Activator.CreateInstance(statType) as Stat;
                        combinedStatBlock.stats.Add(combinedStat);
                    }

                    combinedStat.value = aggregate != 0 ? aggregate : baseValue;
                    combinedStat.combineType = new BaseStat();
                }
            }
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
    }

    public void UpdateSources(IEnumerable<StatBlock> statBlocks)
    {
        UpkeepSources(statBlocks);
        CombineSources();
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

        bool dirty = false;

        //Cleanup any sources not provided this frame
        foreach (var source in sources)
        {
            if (!source.active)
            {
                CleanupSource(source);
                dirty = true;
            }
        }

        if(dirty)
        {
            //combinedStatBlock.stats.Clear();
        }

        sources.RemoveWhere(x => !x.active);

#if UNITY_EDITOR
        sourcesList = sources.ToList();
#endif
    }

    private void CombineSources()
    {
        orderedStats.CombineStats(combinedStatBlock);
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
