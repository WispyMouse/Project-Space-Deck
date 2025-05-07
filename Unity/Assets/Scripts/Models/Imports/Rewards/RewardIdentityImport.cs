namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using static SpaceDeck.GameState.Minimum.RewardPrototype;

    [System.Serializable]
    public class RewardIdentityImport
    {
        public RewardIdentityKind IdentityKind = RewardIdentityKind.Card;
        public string RewardIdentifier = string.Empty;
        public int RewardAmount = 1;
    }
}