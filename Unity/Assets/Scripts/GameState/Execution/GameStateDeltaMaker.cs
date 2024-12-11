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
        public static bool TryCreateDelta(LinkedTokenList linkedTokenList, GameState stateToApplyTo, out GameStateDelta delta)
        {
            return TryCreateDelta(linkedTokenList, new ExecutionAnswerSet(), stateToApplyTo, out delta);
        }

        public static bool TryCreateDelta(LinkedTokenList linkedTokenList, ExecutionAnswerSet answers, GameState stateToApplyTo, out GameStateDelta delta)
        {
            delta = new GameStateDelta();

            ExecutionContext executionContext = new ExecutionContext(stateToApplyTo, linkedTokenList);

            LinkedToken nextToken = linkedTokenList.First;
            while (nextToken != null)
            {
                // TODO: Check conditional for permission to be inside scope before applying
                if (true)
                {
                    if (!nextToken.TryGetChanges(stateToApplyTo, answers, out List<GameStateChange> changes))
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