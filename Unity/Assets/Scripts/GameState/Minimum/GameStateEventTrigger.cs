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
        /// Execution represents things like, "I have an action when I am clicked on"
        /// </summary>
        public enum TriggerDirection
        {
            Unknown = 0,
            Before = 1,
            After = 2,
            Execution = 3
        }

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

        /// <summary>
        /// The card that this trigger relates to.
        /// Can be null.
        /// </summary>
        public CardInstance ProccingCard { get; set; }

        public AppliedStatusEffect ProccingStatusEffect { get; set; }

        public GameStateEventTrigger(LowercaseString eventId)
        {
            this.EventId = eventId;
            this.BasedOnGameStateChange = null;
            this.BasedOnTarget = null;
            this.ProccingCard = null;
            this.ProccingStatusEffect = null;
        }

        public GameStateEventTrigger(LowercaseString eventId, GameStateChange basedOnChange) : this(eventId)
        {
            this.BasedOnGameStateChange = basedOnChange;
            this.BasedOnTarget = this.BasedOnGameStateChange.Target;
        }

        public GameStateEventTrigger(LowercaseString eventId, IChangeTarget basedOnTarget) : this(eventId)
        {
            this.BasedOnTarget = basedOnTarget;
        }

        public GameStateEventTrigger(LowercaseString eventId, CardInstance basedOnCard) : this(eventId)
        {
            this.ProccingCard = basedOnCard;
        }

        public GameStateEventTrigger(LowercaseString eventId, AppliedStatusEffect basedOnStatusEffect) : this(eventId)
        {
            this.ProccingStatusEffect = basedOnStatusEffect;
        }

        public void Apply(IGameStateMutator mutator)
        {
            // TODO: Apply trigger
        }
    }
}