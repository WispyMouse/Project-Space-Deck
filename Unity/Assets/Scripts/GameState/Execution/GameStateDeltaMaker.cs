namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class GameStateDeltaMaker
    {

        public static bool TryCreateDelta(ContextualizedTokenList contextualizedTokens, GameState stateToApplyTo, out GameStateDelta delta)
        {
            return TryCreateDelta(contextualizedTokens, new Dictionary<LinkedExecutionQuestion, LinkedExecutionAnswer>(), stateToApplyTo, out delta);
        }

        public static bool TryCreateDelta(ContextualizedTokenList contextualizedTokens, Dictionary<LinkedExecutionQuestion, LinkedExecutionAnswer> answers, GameState stateToApplyTo, out GameStateDelta delta)
        {
            delta = new GameStateDelta();

            if (!contextualizedTokens.AllAnswersAccountedFor(answers))
            {
                return false;
            }

            ExecutionContext executionContext = new ExecutionContext();

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