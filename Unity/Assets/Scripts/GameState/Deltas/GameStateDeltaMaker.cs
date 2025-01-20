namespace SpaceDeck.GameState.Deltas
{
    using SpaceDeck.GameState.Context;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class GameStateDeltaMaker
    {
        public static bool TryCreateDelta(LinkedTokenList linkedTokenList, IGameStateMutator stateToApplyTo, out GameStateDelta delta)
        {
            return TryCreateDelta(linkedTokenList, new ExecutionAnswerSet(user: null), stateToApplyTo, out delta);
        }

        public static bool TryCreateDelta(LinkedTokenList linkedTokenList, ExecutionAnswerSet answers, IGameStateMutator stateToApplyTo, out GameStateDelta delta, CardInstance playedCard = null, GameStateChange basedOnChange = null, AppliedStatusEffect statusEffect = null)
        {
            delta = new GameStateDelta(stateToApplyTo);

            // If we have a specific played card, try to get elemental gain from it first
            if (playedCard != null && playedCard.ElementalGain != null)
            {
                foreach (LowercaseString element in playedCard.ElementalGain.Keys)
                {
                    delta.ModifyElement(element, playedCard.ElementalGain[element]);
                }
            }

            ScriptingExecutionContext executionContext;
            
            if (playedCard != null)
            {
                executionContext= new ScriptingExecutionContext(delta, linkedTokenList, answers, playedCard);
            }
            else if (statusEffect != null)
            {
                executionContext = new ScriptingExecutionContext(delta, linkedTokenList, answers, statusEffect);
            }
            else
            {
                executionContext = new ScriptingExecutionContext(delta, linkedTokenList, answers);
            }
            executionContext.CurrentlyExecutingGameStateChange = basedOnChange;

            LinkedToken nextToken = linkedTokenList.First;
            while (nextToken != null)
            {
                // TODO: Check conditional for permission to be inside scope before applying
                if (true)
                {
                    if (!nextToken.TryGetChanges(executionContext, out Stack<GameStateChange> changes))
                    {
                        Logging.DebugLog(WellknownLoggingLevels.Error,
                            WellknownLoggingCategories.TryCreateDelta,
                            $"Failed to get changes for executing token.");
                        return false;
                    }

                    if (changes != null)
                    {
                        // Iterate across each change, one at a time, so that it can be determined
                        // if there are any AppliedRules that result from this, they should be applied
                        // and then continued to process
                        // Note the double stack flip; this is to get our copy in the right order. Stacks are funny that way.
                        Stack<GameStateChange> changeStack = new Stack<GameStateChange>(new Stack<GameStateChange>(changes));

                        while (changeStack.Count > 0)
                        {
                            GameStateChange currentlyApplyingChange = changeStack.Pop();

                            executionContext.CurrentlyExecutingGameStateChange = currentlyApplyingChange;

                            if (!currentlyApplyingChange.Triggered)
                            {
                                currentlyApplyingChange.Trigger(delta);
                                currentlyApplyingChange.Triggered = true;
                            }
                            else
                            {
                                if (currentlyApplyingChange.ShouldKeepHistory)
                                {
                                    delta.Changes.Add(currentlyApplyingChange);
                                }

                                currentlyApplyingChange.Apply(delta);
                            }

                            // First stack any triggered events that resulted from the above application
                            // Check if there's any pending resolves, and try to apply them
                            Stack<GameStateChange> resolveChanges = new Stack<GameStateChange>();
                            while (delta.TryGetNextResolve(out IResolve currentResolve))
                            {
                                // If this next resolve already is a GameStateChange, then just push it as is
                                // Otherwise, wrap it in a ResolveChange
                                if (currentResolve is GameStateChange change)
                                {
                                    resolveChanges.Push(change);
                                }
                                else
                                {
                                    resolveChanges.Push(new ResolveChange(currentResolve));
                                }
                            }

                            foreach (GameStateChange resolve in resolveChanges)
                            {
                                changeStack.Push(resolve);
                            }

                            // Then stack rules that are at RuleApplication level
                            List<GameStateChange> rules = RuleReference.GetAppliedRules(delta, new GameStateEventTrigger(WellknownGameStateEvents.RuleApplication, currentlyApplyingChange));
                            if (rules != null && rules.Count > 0)
                            {
                                foreach (GameStateChange rule in rules)
                                {
                                    changeStack.Push(rule);
                                }
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