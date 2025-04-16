namespace SpaceDeck.GameState.Deltas
{
    using SpaceDeck.GameState.Context;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// This class represents for applying game states.
    /// 
    /// At the beginning of every application, it creates a clone
    /// of the entire input game state, modifies its values, and
    /// returns the clone.
    /// 
    /// This allows you to make theoretical game state differences,
    /// which may be used during the creation of the delta itself along
    /// the steps that are available.
    /// </summary>
    public static class GameStateDeltaApplier
    {
        public delegate void GameStateUpdatedDelegate(IGameStateMutator updatedState, GameStateDelta appliedDelta);
        public static event GameStateUpdatedDelegate GameStateUpdated;

        public static void ApplyResolve(IGameStateMutator originalState, IResolve toResolve)
        {
            toResolve.Apply(originalState);
            GameStateUpdated.Invoke(originalState, null);
        }

        public static void ApplyGameStateDelta(IGameStateMutator originalState, GameStateDelta delta)
        {
            foreach (GameStateChange change in delta.Changes)
            {
                change.Apply(originalState);
            }
            GameStateUpdated.Invoke(originalState, delta);
        }
    }
}