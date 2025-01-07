namespace SpaceDeck.Models.Instances
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

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


            }

            return applications.Count > 0;
        }
    }
}