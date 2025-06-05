namespace SpaceDeck.Models.Instances
{
    using System;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;

    public class LinkedShopEntry
    {
        public readonly IEnumerable<IShopCost> Costs;

        public readonly LinkedRewardInstance LinkedGainedReward;

        public LinkedShopEntry(IEnumerable<IShopCost> costs, LinkedRewardInstance rewardInstance)
        {
            this.LinkedGainedReward = rewardInstance;
            this.Costs = costs;
        }

        public int GetGainedAmount(IGameStateMutator mutator)
        {
            return this.LinkedGainedReward.GainedCards.Count + (this.LinkedGainedReward.GainedCurrency.Count >= 1 ? 1 : 0);
        }
    }
}
