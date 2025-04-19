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
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class PlayerTurnEndDiscardRule : Rule
    {
        public PlayerTurnEndDiscardRule() : base(triggerOneventId: WellknownGameStateEvents.EntityTurnEnded, priorityOrder: 1)
        {

        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, TriggerDirection direction, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            if (direction != TriggerDirection.After)
            {
                applications = null;
                return false;
            }

            // Check if the entity that is ending their turn is the player
            // HACK: Assume there is one player!
            IReadOnlyList<Entity> entities = new List<Entity>(trigger.BasedOnTarget.GetRepresentedEntities(gameStateMutator));
            if (entities.Count != 1)
            {
                applications = null;
                return false;
            }

            if (gameStateMutator.GetNumericQuality(entities[0], WellknownQualities.Faction) != WellknownFactions.Player)
            {
                applications = null;
                return false;
            }

            applications = new List<GameStateChange>();

            foreach (CardInstance curCard in gameStateMutator.GetCardsInZone(WellknownZones.Hand))
            {
                applications.Add(new MoveCard(curCard, gameStateMutator.GetStringQuality(curCard, WellknownQualities.Destination, WellknownZones.Discard)));
            }

            return true;
        }
    }
}