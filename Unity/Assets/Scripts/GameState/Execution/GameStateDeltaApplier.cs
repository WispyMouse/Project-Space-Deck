namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
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
        public static GameState ApplyGameStateDelta(GameState originalState, GameStateDelta delta, ExecutionContext executionContext)
        {
            GameState clonedState = originalState.GetClone();

            foreach (GameStateChange change in delta.Changes)
            {
                change.ApplyToGameState(ref clonedState, ref executionContext);
            }

            return clonedState;
        }
    }
}