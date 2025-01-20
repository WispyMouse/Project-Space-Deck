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

        /// <summary>
        /// TriggerAndResolve should not keep the history in GameStateDelta.
        /// They are entirely used to create a GameStateDelta, and are not
        /// useful outside of its limited context.
        /// </summary>
        public bool ShouldKeepHistory => false;

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