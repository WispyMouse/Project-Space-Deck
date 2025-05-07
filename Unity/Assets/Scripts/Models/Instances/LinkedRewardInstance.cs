namespace SpaceDeck.Models.Instances
{
    using System;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;

    public class LinkedRewardInstance
    {
        public readonly RewardPrototype Prototype;
        public readonly IReadOnlyList<LinkedCardInstance> GainedCards = new List<LinkedCardInstance>();
        public readonly IReadOnlyDictionary<Currency, int> GainedCurrency = new Dictionary<Currency, int>();
        // TODO: GAINED ARTIFACT

        public LinkedRewardInstance(RewardPrototype prototype, IReadOnlyList<LinkedCardInstance> gainedCards = null, IReadOnlyDictionary<Currency, int> gainedCurrency = null)
        {
            this.Prototype = prototype;
            this.GainedCards = gainedCards ?? Array.Empty<LinkedCardInstance>();
            this.GainedCurrency = gainedCurrency ?? new Dictionary<Currency, int>();
        }
    }
}
