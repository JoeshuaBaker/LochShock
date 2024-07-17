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

        public void RemoveStat(Stat stat)
        {
            Type statType = stat.GetType();
            StatCombineType combineType = stat.combineType;

            if (orderedStats.ContainsKey(statType) && orderedStats[statType].ContainsKey(combineType))
            {
                HashSet<Stat> statBucket = orderedStats[statType][combineType];
                statBucket.Remove(stat);
            }
        }

        public void CombineStats(NewStatBlock combinedStatBlock)
        {
            foreach(SortedList<StatCombineType, HashSet<Stat>> sortedStats in orderedStats.Values)
            {
                float aggregate = 0;
                Type statType = null;

                foreach(HashSet<Stat> statBucket in sortedStats.Values)
                {
                    if(statBucket.Count != 0)
                    {
                        Stat first = statBucket.First();
                        if(statType == null)
                        {
                            statType = first.GetType();
                        }
                        aggregate = first.combineType.Combine(aggregate, statBucket);
                    }
                }
                
                if(statType != null && aggregate != 0)
                {
                    Stat combinedStat = combinedStatBlock.GetStat(statType);
                    if (combinedStat == null)
                    {
                        combinedStat = Activator.CreateInstance(statType) as Stat;
                        combinedStatBlock.stats.Add(combinedStat);
                    }

                    combinedStat.value = aggregate;
                    combinedStat.combineType = new BaseStat();
                }
            }
        }
    }

    public NewStatBlock combinedStatBlock;
#if UNITY_EDITOR
    //Debug serializable list of sources to cross-check the combined stat block
    public List<NewStatBlock> sourcesList;
#endif
    private HashSet<NewStatBlock> sources;
    private OrderedStats orderedStats;

    public CombinedStatBlock()
    {
        sources = new HashSet<NewStatBlock>();
        orderedStats = new OrderedStats();
        combinedStatBlock = new NewStatBlock();
    }

    private void AddSource(NewStatBlock block)
    {
        sources.Add(block);

        foreach(Stat stat in block.stats)
        {
            orderedStats.AddStat(stat);
        }
    }

    private void CleanupSource(NewStatBlock block)
    {
        foreach (Stat stat in block.stats)
        {
            orderedStats.RemoveStat(stat);
        }
    }

    public void UpdateSources(IEnumerable<NewStatBlock> statBlocks)
    {
        UpkeepSources(statBlocks);
        CombineSources();
    }

    private void UpkeepSources(IEnumerable<NewStatBlock> statBlocks)
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

    private void CombineSources()
    {
        orderedStats.CombineStats(combinedStatBlock);
    }

    public StatBlockContext GetCombinedContext()
    {
        StatBlockContext statBlockContext = new StatBlockContext();

        foreach (NewStatBlock statBlock in sources)
        {
            statBlock.UpdateStatBlockContext(ref statBlockContext);
        }

        return statBlockContext;
    }
}
