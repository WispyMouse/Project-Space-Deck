namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a set of changes for a gamestate.
    /// Combines all of the changes into one set of flat
    /// adjustments. Can be used to predict what will
    /// happen when an effect is executed.
    /// </summary>
    public class GameStateDeltaMaker
    {
        public static bool TryCreateDelta(ContextualizedTokenSet contextualizedTokens, GameState stateToApplyTo, out GameStateDelta delta)
        {
            delta = null;
            return false;
        }
    }
}