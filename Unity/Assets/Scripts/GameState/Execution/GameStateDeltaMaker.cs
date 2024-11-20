namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public static class GameStateDeltaMaker
    {
        public static bool TryCreateDelta(ContextualizedTokenList contextualizedTokens, GameState stateToApplyTo, out GameStateDelta delta)
        {
            delta = null;
            return false;
        }
    }
}