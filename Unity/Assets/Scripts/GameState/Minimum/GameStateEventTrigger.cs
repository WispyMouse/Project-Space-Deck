namespace SpaceDeck.GameState.Minimum
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    public struct GameStateEventTrigger
    {
        public LowercaseString EventId;

        /// <summary>
        /// The event that directly caused this trigger.
        /// Can be null.
        /// </summary>
        public GameStateChange BasedOnGameStateChange { get; set; }

        public GameStateEventTrigger(LowercaseString eventId)
        {
            this.EventId = eventId;
            this.BasedOnGameStateChange = null;
        }

        public GameStateEventTrigger(LowercaseString eventId, GameStateChange basedOnChange)
        {
            this.EventId = eventId;
            this.BasedOnGameStateChange = basedOnChange;
        }
    }
}