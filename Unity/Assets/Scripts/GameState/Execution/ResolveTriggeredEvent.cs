namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    public class ResolveTriggeredEvent : IResolve
    {
        public readonly AppliedStatusEffect TriggeredEffect;
        public readonly GameStateEventTrigger Trigger;

        public ResolveTriggeredEvent(AppliedStatusEffect triggeredStatusEffect, GameStateEventTrigger trigger)
        {
            this.TriggeredEffect = triggeredStatusEffect;
            this.Trigger = trigger;
        }

        public void Apply(IGameStateMutator mutator)
        {
            if (this.TriggeredEffect.TryApplyStatusEffect(this.Trigger, mutator, out List<GameStateChange> applications))
            {
                foreach (GameStateChange change in applications)
                {
                    mutator.PushResolve(change);
                }
            }
        }
    }
}