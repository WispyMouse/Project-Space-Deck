namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Context;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
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
            delta = new GameStateDelta(stateToApplyTo);

            ScriptingExecutionContext executionContext = new ScriptingExecutionContext(stateToApplyTo, linkedTokenList, answers);

            LinkedToken nextToken = linkedTokenList.First;
            while (nextToken != null)
            {
                // TODO: Check conditional for permission to be inside scope before applying
                if (true)
                {
                    if (!nextToken.TryGetChanges(executionContext, out List<GameStateChange> changes))
                    {
                        // TODO LOG FAILURE
                        return false;
                    }

                    if (changes != null)
                    {
                        // Iterate across each change, one at a time, so that it can be determined
                        // if there are any AppliedRules that result from this, they should be applied
                        // and then continued to process
                        List<GameStateChange> changeStack = new List<GameStateChange>(changes);

                        for (int ii = 0; ii < changeStack.Count; ii++)
                        {
                            delta.Changes.Add(changeStack[ii]);
                            changeStack[ii].ApplyToGameState(delta);

                            List<GameStateChange> rules = RuleReference.GetAppliedRules(executionContext, delta, changeStack[ii]);
                            if (rules != null && rules.Count > 0)
                            {
                                changeStack.InsertRange(ii+1, rules);
                            }
                        }
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