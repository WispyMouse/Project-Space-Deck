namespace SpaceDeck.GameState.Rules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Utility.Wellknown;

    public class EncounterStartPlayerTurnRule : Rule
    {
        public EncounterStartPlayerTurnRule() : base(WellknownGameStateEvents.EncounterStart)
        {

        }
    }
}