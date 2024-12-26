namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    /// <summary>
    /// Describes something that mutates the game state upon execution.
    /// </summary>
    public interface IResolve
    {
        void Apply(IGameStateMutator mutator);
    }
}