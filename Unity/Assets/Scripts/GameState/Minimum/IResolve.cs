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

        /// <summary>
        /// Indicates if this IResolve should be maintained when a
        /// GameStateDelta is assembled. Maintaining the history of
        /// some IResolve results in unnecessary logistical supporting
        /// items being in the stack.
        /// </summary>
        bool ShouldKeepHistory { get; }
    }
}