namespace SpaceDeck.GameState.Minimum
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    public struct GameStateEventTrigger : IResolve
    {
        /// <summary>
        /// Describes if this trigger is triggering *before* the action it is related to is committed,
        /// or if it is triggering *after* the action.
        /// For example, "I should take less damage from an incoming attack" should react before.
        /// "I gain stacks based on damage I've taken" should react after.
        /// </summary>
        public enum TriggerDirection
        {
            Before,
            After
        }

        public LowercaseString EventId;

        public TriggerDirection TriggeredDirection;

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

        /// <summary>
        /// The card that this trigger relates to.
        /// Can be null.
        /// </summary>
        public CardInstance ProccingCard { get; set; }

        public GameStateEventTrigger(LowercaseString eventId, TriggerDirection direction)
        {
            this.TriggeredDirection = direction;
            this.EventId = eventId;
            this.BasedOnGameStateChange = null;
            this.BasedOnTarget = null;
            this.ProccingCard = null;
        }

        public GameStateEventTrigger(LowercaseString eventId, GameStateChange basedOnChange, TriggerDirection direction) : this(eventId, direction)
        {
            this.BasedOnGameStateChange = basedOnChange;
            this.BasedOnTarget = this.BasedOnGameStateChange.Target;
        }

        public GameStateEventTrigger(LowercaseString eventId, IChangeTarget basedOnTarget, TriggerDirection direction) : this(eventId, direction)
        {
            this.BasedOnTarget = basedOnTarget;
        }

        public GameStateEventTrigger(LowercaseString eventId, CardInstance basedOnCard, TriggerDirection direction) : this(eventId, direction)
        {
            this.BasedOnTarget = null;
            this.BasedOnGameStateChange = null;
            this.ProccingCard = basedOnCard;
        }

        public void Apply(IGameStateMutator mutator)
        {
            // TODO: Apply trigger
        }
    }
}