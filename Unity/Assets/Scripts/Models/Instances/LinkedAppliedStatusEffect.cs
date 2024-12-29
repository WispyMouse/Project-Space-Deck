namespace SpaceDeck.Models.Instances
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;

    public class LinkedAppliedStatusEffect : AppliedStatusEffect
    {
        public readonly StatusEffectPrototype Prototype;

        public LinkedAppliedStatusEffect(StatusEffectPrototype prototype) : base(prototype.Id, prototype.Name)
        {
            if (prototype.Reactors != null)
            {
                foreach (Reactor reactor in prototype.Reactors)
                {
                    this.TriggerOnEventIds.UnionWith(reactor.ReactionTriggers);
                }
            }
        }
    }
}