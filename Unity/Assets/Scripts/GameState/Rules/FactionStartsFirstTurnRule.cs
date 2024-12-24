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

    public class FactionStartsFirstTurnRule : Rule
    {
        public FactionStartsFirstTurnRule() : base(WellknownGameStateEvents.FactionTurnStarted)
        {
        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            gameStateMutator.EntityTurnTakerCalculator = new EntityTurnTakerCalculator(gameStateMutator, gameStateMutator.FactionTurnTakerCalculator.GetCurrentFaction());
            if (!gameStateMutator.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(gameStateMutator, out Entity firstTurn))
            {
                applications = null;
                return false;
            }

            applications = new List<GameStateChange>() { new StartEntityTurn(firstTurn) };
            return true;
        }
    }
}