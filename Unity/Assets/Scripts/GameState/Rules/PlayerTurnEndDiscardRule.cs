namespace SpaceDeck.GameState.Rules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Utility.Wellknown;

    public class PlayerTurnEndDiscardRule : Rule
    {
        public PlayerTurnEndDiscardRule() : base(triggerOneventId: WellknownGameStateEvents.EntityTurnEnded, priorityOrder: 1)
        {

        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            if (!gameStateMutator.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(gameStateMutator, out Entity currentTurn))
            {
                applications = null;
                return false;
            }

            if (currentTurn.GetQuality(WellknownQualities.Faction) != WellknownFactions.Player)
            {
                applications = null;
                return false;
            }

            applications = new List<GameStateChange>();

            foreach (CardInstance curCard in gameStateMutator.GetCardsInZone(WellknownZones.Hand))
            {
                applications.Add(new MoveCard(curCard, WellknownZones.Discard));
            }

            return true;
        }
    }
}