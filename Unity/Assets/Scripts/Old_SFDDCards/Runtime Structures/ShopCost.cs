namespace SFDDCards
{
    using SFDDCards.ImportModels;
    using SFDDCards.ScriptingTokens.EvaluatableValues;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class ShopCost
    {
        public IEvaluatableValue<int> Amount;

        [Obsolete("Should transition to " + nameof(_CurrencyType))]
        public SFDDCards.ImportModels.CurrencyImport CurrencyType;
        public Currency _CurrencyType;
    }
}
