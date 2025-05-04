namespace SpaceDeck.Utility.Wellknown
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    public static class WellknownCampaignStates
    {
        public static readonly LowercaseString ChoosingRouteNode = nameof(ChoosingRouteNode);
        public static readonly LowercaseString ShopEncounter = nameof(ShopEncounter);
        public static readonly LowercaseString DialogueEncounter = nameof(DialogueEncounter);
        public static readonly LowercaseString CombatEncounter = nameof(CombatEncounter);
        public static readonly LowercaseString ChoosingRewards = nameof(ChoosingRewards);
        public static readonly LowercaseString GameOver = nameof(GameOver);
        public static readonly LowercaseString EncounterResolved = nameof(EncounterResolved);
    }
}