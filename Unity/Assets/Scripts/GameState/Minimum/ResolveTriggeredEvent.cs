namespace SpaceDeck.GameState.Minimum
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class ResolveTriggeredEvent : IResolve
    {
        public readonly AppliedStatusEffect TriggeredEffect;
        public readonly GameStateEventTrigger Trigger;
        public readonly TriggerDirection Direction;

        public ResolveTriggeredEvent(AppliedStatusEffect triggeredStatusEffect, GameStateEventTrigger trigger, TriggerDirection direction)
        {
            this.TriggeredEffect = triggeredStatusEffect;
            this.Trigger = trigger;
            this.Direction = direction;
        }

        public void Apply(IGameStateMutator mutator)
        {
            if (this.TriggeredEffect.TryApplyStatusEffect(this.Trigger, mutator, this.Direction, out List<GameStateChange> applications))
            {
                foreach (GameStateChange change in applications)
                {
                    mutator.PushResolve(change);
                }
            }
        }
    }
}