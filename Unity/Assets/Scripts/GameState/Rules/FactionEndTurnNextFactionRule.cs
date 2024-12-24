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

    public class FactionEndTurnNextFactionRule : Rule
    {
        public FactionEndTurnNextFactionRule() : base(WellknownGameStateEvents.FactionTurnEnded)
        {
        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            applications = new List<GameStateChange>() { new StartFactionTurn(gameStateMutator.FactionTurnTakerCalculator.GetNextFactionTurn(gameStateMutator)) };
            return true;
        }
    }
}