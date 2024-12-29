using SpaceDeck.Utility.Minimum;
using System.Collections.Generic;

namespace SpaceDeck.GameState.Minimum
{
    public interface IEffectIDescribable : IDescribable
    {
        EffectDescription GetDescription();
    }
}