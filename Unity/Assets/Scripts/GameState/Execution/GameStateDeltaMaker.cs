namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.GameState.Context;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class GameStateDeltaMaker
    {
        public static bool TryCreateDelta(LinkedTokenList linkedTokenList, GameState stateToApplyTo, out GameStateDelta delta)
        {
            return TryCreateDelta(linkedTokenList, new ExecutionAnswerSet(), stateToApplyTo, out delta);
        }

        public static bool TryCreateDelta(LinkedTokenList linkedTokenList, ExecutionAnswerSet answers, GameState stateToApplyTo, out GameStateDelta delta, CardInstance playedCard = null)
        {
            delta = new GameStateDelta(stateToApplyTo);

            ScriptingExecutionContext executionContext = new ScriptingExecutionContext(stateToApplyTo, linkedTokenList, answers, playedCard);

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
                            changeStack[ii].Apply(delta);

                            // First stack any triggered events that resulted from the above application
                            // Check if there's any pending resolves, and try to apply them
                            while (delta.TryGetNextResolve(out IResolve currentResolve))
                            {
                                changeStack.Insert(ii+1, new ResolveChange(currentResolve));
                            }

                            // Then stack rules that are at RuleApplication level
                            List<GameStateChange> rules = RuleReference.GetAppliedRules(delta, new GameStateEventTrigger(WellknownGameStateEvents.RuleApplication, changeStack[ii], GameStateEventTrigger.TriggerDirection.After));
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
                    // nextToken = nextToken.LinkedScope.NextStatementAfterScope;
                }
            }

            return true;
        }
    }
}