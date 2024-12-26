namespace SpaceDeck.GameState.Minimum
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    public struct GameStateEventTrigger : IResolve
    {
        public LowercaseString EventId;

        /// <summary>
        /// The event that directly caused this trigger.
        /// Can be null.
        /// </summary>
        public GameStateChange BasedOnGameStateChange { get; set; }

        /// <summary>
        /// The target this trigger is in direct relation to.
        /// When designing triggers, designers should think as though the target "owns" this trigger
        /// Can be null.
        /// </summary>
        public IChangeTarget BasedOnTarget { get; set; }

        public GameStateEventTrigger(LowercaseString eventId)
        {
            this.EventId = eventId;
            this.BasedOnGameStateChange = null;
            this.BasedOnTarget = null;
        }

        public GameStateEventTrigger(LowercaseString eventId, GameStateChange basedOnChange)
        {
            this.EventId = eventId;
            this.BasedOnGameStateChange = basedOnChange;
            this.BasedOnTarget = this.BasedOnGameStateChange.Target;
        }

        public GameStateEventTrigger(LowercaseString eventId, IChangeTarget basedOnTarget)
        {
            this.EventId = eventId;
            this.BasedOnGameStateChange = null;
            this.BasedOnTarget = basedOnTarget;
        }

        public void Apply(IGameStateMutator mutator)
        {
            // TODO: Apply trigger
        }
    }
}