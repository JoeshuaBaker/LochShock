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

    public struct StatContext
    {
        //Text for the main body which is being displayed. Instances of the value should be included as %value%
        public string text;

        //Text which will be prefixed to the value whenever it's printed in the tooltip (e.g +, x)
        public string valuePrefix;

        //Text which will be postfixed to the value whenever it's printed in the tooltip (good for the name of the value)
        public string valuePostfix;

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

            string formattedValueString = $"{valuePrefix}{(isPercentage ? formattedValue.ToString("P0").Replace(" ", "") : formattedValue.ToString("0.#"))} {valuePostfix}";

            return $"{highlight}{text.Replace("%value%", formattedValueString)}</color>";
        }

        private static string GetPrefixByBlockType(StatBlock.BlockType type, string valueName, bool flipSign, float value)
        {
            bool plusMinus;
            switch (type)
            {
                case StatBlock.BlockType.Set:
                    return $"{valueName.SplitCamelCaseLower()} is always ";

                default:
                case StatBlock.BlockType.Base:
                    return "";

                case StatBlock.BlockType.PlusMult:
                case StatBlock.BlockType.Additive:
                    plusMinus = value >= 0;
                    if(flipSign)
                    {
                        plusMinus = !plusMinus;
                    }

                    return plusMinus ? "+" : "-";

                case StatBlock.BlockType.xMult:
                    plusMinus = value >= 0;
                    if(flipSign)
                    {
                        plusMinus = !plusMinus;
                    }
                    
                    return plusMinus ? "x" : "-x";
            }
        }

        private static string GetPostfixByBlockType(StatBlock.BlockType type, string valueName)
        {
            switch (type)
            {
                case StatBlock.BlockType.Set:
                    return $"";

                default:
                case StatBlock.BlockType.Base:
                case StatBlock.BlockType.PlusMult:
                case StatBlock.BlockType.Additive:
                case StatBlock.BlockType.xMult:
                    return valueName.SplitCamelCaseLower();
            }
        }

        public StatContext(
            StatBlock.BlockType blockType, 
            string valueName, 
            float value,
            bool isPercentage = false,
            bool positiveIsGood = true,
            bool flipSign = false, 
            string text = "%value%")
        {
            this.value = value;
            this.text = text;
            this.isBaseStatBlock = blockType == StatBlock.BlockType.Base;
            this.isPercentage = isPercentage || blockType == StatBlock.BlockType.PlusMult || blockType == StatBlock.BlockType.xMult;
            this.positiveIsGood = positiveIsGood;
            this.valuePrefix = GetPrefixByBlockType(blockType, valueName, flipSign, value);
            this.valuePostfix = GetPostfixByBlockType(blockType, valueName);
        }
    }


    private Dictionary<string, StatContext> statDictionary = new Dictionary<string, StatContext>();

    public void AddContext(
        string key,
        StatBlock.BlockType blockType,
        string valueName,
        float value,
        bool isPercentage = false,
        bool positiveIsGood = true,
        bool flipSign = false,
        string text = "%value%",
        float baseValue = 0f)
    {
        if(value != 0 && !(blockType == StatBlock.BlockType.Base && value == baseValue))
        {
            string typeKey = key + (int)blockType;
            bool contextExists = statDictionary.TryGetValue(typeKey, out StatContext existingContext);

            if (contextExists)
            {
                existingContext.value += value;
                statDictionary[typeKey] = existingContext;
            }
            else
            {
                statDictionary.Add(typeKey, new StatContext(blockType, valueName, value, isPercentage, positiveIsGood, flipSign, text));
            }
        }
    }

    public IEnumerable<string> GetStatContextStrings()
    {
        return statDictionary.Values.Select(x => x.GetFormattedText());
    }
}
