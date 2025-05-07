using System.Collections.Generic;

namespace SpaceDeck.GameState.Minimum
{
    public interface IShopEntry
    {
        IReadOnlyList<IShopCost> Costs { get; }

        RewardPrototype GainedReward { get; }

        int GetGainedAmount(IGameStateMutator mutator);
    }
}