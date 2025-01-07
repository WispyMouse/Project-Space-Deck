namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.GameState.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class TriggerAndResolve : IResolve
    {
        public readonly GameStateEventTrigger Trigger;
        public readonly TriggerDirection Direction;

        public TriggerAndResolve(GameStateEventTrigger trigger, TriggerDirection direction)
        {
            this.Trigger = trigger;
            this.Direction = direction;
        }

        public void Apply(IGameStateMutator mutator)
        {
            IReadOnlyList<IResolve> resolves = mutator.GetTriggers(this.Trigger, this.Direction);
            foreach (IResolve resolve in resolves)
            {
                mutator.PushResolve(resolve);
            }
        }
    }
}