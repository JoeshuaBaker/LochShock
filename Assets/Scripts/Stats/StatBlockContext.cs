using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatBlockContext
{
    public static string NeutralColor = $"<color=#{ColorUtility.ToHtmlStringRGB(Color.white)}>";
    public static string GoodColor = $"<color=#{ColorUtility.ToHtmlStringRGB(Color.green)}>";
    public static string BadColor = $"<color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>";
    public static string HighlightColor = $"<color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>";
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
            string highlight;

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

            return $"{highlight}{text.Replace(VALUE, formattedValueString)}</color>";
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
    private List<string> genericTooltips = new List<string>();
    private List<string> allTooltips = new List<string>();

    public void AddContext(
        string key,
        StatCombineType blockType,
        string valueName,
        float value,
        bool isPercentage = false,
        bool positiveIsGood = true,
        bool flipSign = false,
        float baseValue = 0f,
        List<StatCondition> conditions = null)
    {
        if(value != 0 && !(blockType is BaseStat && value == baseValue))
        {
            string typeKey = key + blockType.CombinePriority;
            bool contextExists = statDictionary.TryGetValue(typeKey, out StatContext existingContext);

            if (contextExists)
            {
                existingContext.value += value;
                statDictionary[typeKey] = existingContext;
            }
            else
            {
                statDictionary.Add(typeKey, new StatContext(blockType, valueName, value, isPercentage, positiveIsGood, flipSign, conditions));
            }
        }
    }

    public void AddGenericTooltip(string tooltip)
    {
        genericTooltips.Add(tooltip);
    }

    public IEnumerable<string> GetStatContextStrings()
    {
        allTooltips.Clear();
        allTooltips.AddRange(genericTooltips);
        allTooltips.AddRange(statDictionary.Values.Select(x => x.GetFormattedText()));
        return allTooltips;
    }
}
