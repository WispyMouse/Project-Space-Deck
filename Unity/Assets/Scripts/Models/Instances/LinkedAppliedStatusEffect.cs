namespace SpaceDeck.Models.Instances
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Deltas;
    using SpaceDeck.Models.Prototypes;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;

    public class LinkedAppliedStatusEffect : AppliedStatusEffect
    {
        public readonly StatusEffectPrototype Prototype;
        public readonly List<Reactor> Reactors;

        public LinkedAppliedStatusEffect(StatusEffectPrototype prototype) : base(prototype.Id, prototype.Name)
        {
            this.Reactors = prototype.Reactors;

            if (prototype.Reactors != null)
            {
                foreach (Reactor reactor in prototype.Reactors)
                {
                    this.TriggerOnEventIds.UnionWith(reactor.ReactionTriggers);
                }
            }
        }

        public override bool TryApplyStatusEffect(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, TriggerDirection direction, out List<GameStateChange> applications)
        {
            applications = new List<GameStateChange>();

            foreach (Reactor reactor in this.Reactors)
            {
                if (!reactor.ReactionTriggers.Contains(trigger.EventId))
                {
                    // Doesn't apply to this reactor
                    continue;
                }

                if (reactor.Direction != direction)
                {
                    // Only react to prescribed direction
                    continue;
                }

                // TODO CHECK IF SHOULD PROC

                if (trigger.BasedOnTarget == null)
                {
                    continue;
                }

                foreach (Entity curEntity in trigger.BasedOnTarget.GetRepresentedEntities(gameStateMutator))
                {
                    ExecutionAnswerSet answers = new ExecutionAnswerSet(curEntity);

                    if (!reactor.LinkedTokens.HasValue)
                    {
                        Logging.DebugLog(WellknownLoggingLevels.Error,
                            WellknownLoggingCategories.LinkingFailure,
                            $"Reactor has null LinkedTokens. After adding all database entries, use AllDatabases.LinkAllDatabase to link them.");
                        return false;
                    }

                    if (!GameStateDeltaMaker.TryCreateDelta(reactor.LinkedTokens.Value, answers, gameStateMutator, out GameStateDelta delta, playedCard: trigger.ProccingCard, basedOnChange: trigger.BasedOnGameStateChange, statusEffect: this))
                    {
                        Logging.DebugLog(WellknownLoggingLevels.Error,
                            WellknownLoggingCategories.TryCreateDelta,
                            $"Failed to create delta with linked tokens.");
                        return false;
                    }

                    applications.AddRange(delta.Changes);
                }

            }

            return applications.Count > 0;
        }
    }
}