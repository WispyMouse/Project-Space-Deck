namespace SFDDCards
{
    using SFDDCards.ImportModels;
    using SFDDCards.ScriptingTokens.EvaluatableValues;
    using SpaceDeck.Models.Instances;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class ShopEntry : IGainable
    {
        public StatusEffect GainedEffect { get; set; }
        public Card GainedCard { get; set; }
        public Currency GainedCurrency { get; set; }

        public IEvaluatableValue<int> GainedAmount { get; set; }

        public List<ShopCost> Costs;
    }
}