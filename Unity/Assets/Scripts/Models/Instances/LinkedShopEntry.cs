namespace SpaceDeck.Models.Instances
{
    using System;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;

    public class LinkedShopEntry : IShopEntry
    {
        public IReadOnlyList<IShopCost> Costs => this._Costs;
        protected readonly IReadOnlyList<IShopCost> _Costs;

        public RewardPrototype GainedReward => this._GainedReward;
        protected readonly RewardPrototype _GainedReward;

        public readonly LinkedRewardInstance LinkedGainedReward;

        public LinkedShopEntry(RewardPrototype rewardPrototype, IReadOnlyList<IShopCost> costs, LinkedRewardInstance rewardInstance)
        {
            this._GainedReward = rewardPrototype;
            this.LinkedGainedReward = rewardInstance;
            this._Costs = costs;
        }

        public int GetGainedAmount(IGameStateMutator mutator)
        {
            return this.GainedReward.RewardAmount;
        }
    }
}
