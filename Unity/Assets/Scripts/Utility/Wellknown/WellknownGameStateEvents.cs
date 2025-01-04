namespace SpaceDeck.Utility.Wellknown
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    public static class WellknownGameStateEvents
    {
        public readonly static LowercaseString RuleApplication = nameof(RuleApplication);
        public readonly static LowercaseString Activated = nameof(Activated);

        public readonly static LowercaseString EncounterStart = nameof(EncounterStart);
        public readonly static LowercaseString FactionTurnStarted = nameof(FactionTurnStarted);
        public readonly static LowercaseString FactionTurnEnded = nameof(FactionTurnEnded);

        public readonly static LowercaseString EntityTurnStarted = nameof(EntityTurnStarted);
        public readonly static LowercaseString EntityTurnEnded = nameof(EntityTurnEnded);
        public readonly static LowercaseString CardPlayed = nameof(CardPlayed);
    }
}