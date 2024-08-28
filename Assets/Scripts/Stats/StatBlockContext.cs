using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatBlockContext
{
    public static Color NeutralColor = Color.white;
    public static Color GoodColor = Color.green;
    public static Color BadColor = Color.red;
    public static Color HighlightColor = Color.yellow;
    private const string VALUE = "%value%";

    public struct StatContext
    {
        //Text for the main body which is being displayed. Instances of the value should be included as %value%
        public string text;

        //Defines whether this is a base stat block or not (should ignore good/bad coloring)
        public bool isBaseStatBlock;

        //Defines whether having positive or negative values of this stat is considered 'good' or 'bad' for text coloring purposes
        public bool positiveIsGood;

        //Defines whether the value should be multiplied by 100 and appended with a percentage sign
        public bool isPercentage;

        //Value, which is formatted/appended into the string, replacing any instances of the word %value%
        public float value;

        public string GetFormattedText()
        {
            Color highlight;

            bool isGood = value >= 0;

            if (isBaseStatBlock)
            {
                highlight = NeutralColor;
            }
            else
            {
                if (!positiveIsGood)
                {
                    isGood = !isGood;
                }

                if (isGood)
                {
                    highlight = GoodColor;
                }
                else
                {
                    highlight = BadColor;
                }
            }

            float formattedValue = Mathf.Abs(value);

            string formattedValueString = $"{(isPercentage ? formattedValue.ToString("P0").Replace(" ", "") : formattedValue.ToString("0.#"))}";

            return $"{text.Replace(VALUE, formattedValueString)}".AddColorToString(highlight);
        }

        public StatContext(
            StatCombineType blockType, 
            string valueName, 
            float value,
            bool isPercentage = false,
            bool positiveIsGood = true,
            bool flipSign = false,
            List<StatCondition> conditions = null)
        {
            this.value = value;
            this.text = $"{blockType.GetTooltipPrefix(valueName, flipSign, value)} {VALUE} {blockType.StatNameTooltip(valueName)}";
            this.text = StatCondition.FormatConditionTooltip(conditions, text);
            this.isBaseStatBlock = blockType is BaseStat;
            this.isPercentage = isPercentage || blockType is Mult;
            this.positiveIsGood = positiveIsGood;
        }
    }


    private Dictionary<string, StatContext> statDictionary = new Dictionary<string, StatContext>();
    private List<string> genericPrefixTooltips = new List<string>();
    private List<string> genericBaseStatSeperatorTooltips = new List<string>();
    private List<string> genericPostfixTooltips = new List<string>();
    private List<string> allTooltips = new List<string>();

    public void AddStatContext(
        Stat stat,
        string valueName,
        bool isPercentage = false,
        bool positiveIsGood = true,
        bool flipSign = false,
        float baseValue = 0f,
        List<StatCondition> conditions = null)
    {
        float value = stat.value * stat.TooltipStacks;
        if(value != 0 && !(stat.combineType is BaseStat && value == baseValue))
        {
            string typeKey = stat.Name() + stat.combineType.CombinePriority;
            bool contextExists = statDictionary.TryGetValue(typeKey, out StatContext existingContext);

            if (contextExists)
            {
                existingContext.value += value;
                statDictionary[typeKey] = existingContext;
            }
            else
            {
                statDictionary.Add(typeKey, new StatContext(stat.combineType, valueName, value, isPercentage, positiveIsGood, flipSign, conditions));
            }
        }
    }

    public void RemoveAllMatchingStatContext(Stat stat)
    {
        string typeKey = stat.Name() + stat.combineType.CombinePriority;
        statDictionary.Remove(typeKey);
    }

    public void AddGenericPrefixTooltip(string tooltip)
    {
        genericPrefixTooltips.Add(tooltip);
    }

    public void AddGenericPostfixTooltip(string tooltip)
    {
        genericPostfixTooltips.Add(tooltip);
    }

    public void AddGenericBaseStatSeperatorTooltip(string tooltip)
    {
        genericBaseStatSeperatorTooltips.Add(tooltip);
    }

    public IEnumerable<string> GetStatContextStrings()
    {
        allTooltips.Clear();
        allTooltips.AddRange(genericPrefixTooltips);
        allTooltips.AddRange(statDictionary.Values.Where(x => x.isBaseStatBlock).Select(x => x.GetFormattedText()));
        allTooltips.AddRange(genericBaseStatSeperatorTooltips);
        allTooltips.AddRange(statDictionary.Values.Where(x => !x.isBaseStatBlock).Select(x => x.GetFormattedText()));
        allTooltips.AddRange(genericPostfixTooltips);
        return allTooltips;
    }
}
