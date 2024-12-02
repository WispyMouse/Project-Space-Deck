namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Context;
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
            return TryCreateDelta(contextualizedTokens, new Dictionary<LinkedToken, ExecutionAnswerSet>(), stateToApplyTo, out delta);
        }

        public static bool TryCreateDelta(ContextualizedTokenList contextualizedTokens, Dictionary<LinkedToken, ExecutionAnswerSet> answers, GameState stateToApplyTo, out GameStateDelta delta)
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
                    if (!answers.TryGetValue(nextToken, out ExecutionAnswerSet answersForToken))
                    {
                        answersForToken = null;
                    }

                    if (!nextToken.TryGetChanges(stateToApplyTo, answersForToken, out List<GameStateChange> changes))
                    {
                        // TODO LOG FAILURE
                        return false;
                    }

                    if (changes != null)
                    {
                        delta.Changes.AddRange(changes);
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