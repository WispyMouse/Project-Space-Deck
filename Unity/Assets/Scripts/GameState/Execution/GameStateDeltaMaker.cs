namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public static class GameStateDeltaMaker
    {
        public static bool TryCreateDelta(ContextualizedTokenList contextualizedTokens, GameState stateToApplyTo, out GameStateDelta delta)
        {
            delta = new GameStateDelta();
            ExecutionContext executionContext = new ExecutionContext();

            // TODO: We're going to assign the target to the first enemy, if one is available
            // This would normally be provided with answers to the contextualized tokens list
            if (stateToApplyTo.PersistentEntities.Count > 0)
            {
                executionContext.CurrentDefaultTarget = stateToApplyTo.PersistentEntities[0];
            }

            LinkedToken nextToken = contextualizedTokens.Tokens.First;
            while (nextToken != null)
            {
                // TODO: Check conditional for permission to be inside scope before applying
                if (true)
                {
                    if (!nextToken.CommandToExecute.TryApplyDelta(executionContext, stateToApplyTo, nextToken, ref delta))
                    {
                        // TODO LOG FAILURE
                        return false;
                    }

                    nextToken = nextToken.NextLinkedToken;
                }
                else
                {
                    nextToken = nextToken.LinkedScope.NextStatementAfterScope;
                }
            }

            return true;
        }
    }
}