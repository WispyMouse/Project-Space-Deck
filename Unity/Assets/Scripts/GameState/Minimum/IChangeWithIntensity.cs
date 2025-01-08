using SpaceDeck.Utility.Minimum;
using System.Collections.Generic;

namespace SpaceDeck.GameState.Minimum
{
    public interface IChangeWithIntensity
    {
        decimal Intensity { get; set; }
        InitialIntensityPositivity Positivity { get; }
    }

    public enum InitialIntensityPositivity
    {
        PositiveOrZero,
        NegativeOrZero
    }
}