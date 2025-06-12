namespace SpaceDeck.Creator
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using JetBrains.Annotations;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.UX;
    using TMPro;
    using UnityEngine;

    public class CardCreatorPanel : MonoBehaviour
    {
        public CardPrototype RepresentedCard { get; private set; }

        [SerializeReference]
        private TMP_Text CardEverythingLabel;

        public void ShowCard(CardPrototype cardPrototype)
        {
            this.gameObject.SetActive(true);
            this.RepresentedCard = cardPrototype;

            /*
             * public readonly LowercaseString Id;
                public ParsedTokenList? ParsedTokens;
                public LinkedTokenList? LinkedTokens;
                public Dictionary<LowercaseString, int> ElementalGain;
                public QualitiesHolder Qualities;
                public HashSet<LowercaseString> Tags;
             * */

            StringBuilder wholeStringBuilder = new StringBuilder();

            wholeStringBuilder.AppendLine(cardPrototype.Id);
            wholeStringBuilder.AppendLine(cardPrototype.LinkedTokens.Value.Describe());

            foreach (LowercaseString element in cardPrototype.ElementalGain.Keys)
            {
                wholeStringBuilder.AppendLine($"{element.ToString()} => {cardPrototype.ElementalGain[element]}");
            }

            IReadOnlyDictionary<LowercaseString, decimal> numericQualities = cardPrototype.Qualities.GetNumericQualities();

            foreach (LowercaseString numericQualityIndex in numericQualities.Keys)
            {
                wholeStringBuilder.AppendLine($"{numericQualityIndex} => {numericQualities[numericQualityIndex]}");
            }

            IReadOnlyDictionary<LowercaseString, string> stringQualities = cardPrototype.Qualities.GetStringQualities();

            foreach (LowercaseString stringQualityIndex in stringQualities.Keys)
            {
                wholeStringBuilder.AppendLine($"{stringQualityIndex} => {stringQualities[stringQualityIndex]}");
            }

            wholeStringBuilder.AppendLine("Tags: ");
            string tagComma = "";
            foreach (LowercaseString tag in cardPrototype.Tags)
            {
                wholeStringBuilder.Append(tagComma + tag);
                tagComma = ", ";
            }
            wholeStringBuilder.Append("\n");

            this.CardEverythingLabel.text = wholeStringBuilder.ToString();
        }
    }
}