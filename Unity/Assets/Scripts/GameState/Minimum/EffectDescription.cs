namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    public class EffectDescription
    {
        public readonly string DescribingLabel = string.Empty;
        public readonly IReadOnlyList<string> DescriptionText = new List<string>();
        public readonly HashSet<IEffectIDescribable> MentionedDescribables = new HashSet<IEffectIDescribable>();

        public EffectDescription(string describingLabel, IReadOnlyList<string> descriptionText, HashSet<IEffectIDescribable> mentionedDescribables)
        {
            this.DescribingLabel = describingLabel;
            this.DescriptionText = descriptionText;
            this.MentionedDescribables = mentionedDescribables;
        }

        public string BreakDescriptionsIntoString()
        {
            StringBuilder descriptionBuilder = new StringBuilder();

            string leadingNewline = "";
            IReadOnlyList<string> descriptionTexts = this.DescriptionText;
            for (int ii = 0; ii < descriptionTexts.Count; ii++)
            {
                descriptionBuilder.Append($"{leadingNewline}{descriptionTexts[ii].Trim()}");
                leadingNewline = "\r\n";
            }

            return descriptionBuilder.ToString().Trim();
        }
    }
}