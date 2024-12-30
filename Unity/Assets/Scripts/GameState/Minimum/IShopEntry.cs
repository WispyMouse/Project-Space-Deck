using System.Collections.Generic;

namespace SpaceDeck.GameState.Minimum
{
    public interface IShopEntry
    {
        List<IShopCost> Costs { get; }

        AppliedStatusEffect GainedArtifact { get; }
        CardInstance GainedCard { get; }
        Currency GainedCurrency { get; }

        int GetGainedAmount(IGameStateMutator mutator);
    }
}