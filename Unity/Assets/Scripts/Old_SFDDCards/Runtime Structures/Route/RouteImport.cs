namespace SFDDCards
{
    using SFDDCards.ImportModels;
    using SpaceDeck.Models.Imports;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class RouteImport : Importable
    {
        public string RouteName { get; set; }
        public List<ChoiceNodeImport> RouteNodes { get; set; } = new List<ChoiceNodeImport>();

        public List<string> StartingDeck = new List<string>();
        public int StartingMaximumHealth = 50;

        public List<CostEvaluationModifier> CostModifiers = new List<CostEvaluationModifier>();
        public List<StartingCurrency> StartingCurrencies = new List<StartingCurrency>();
    }
}