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

    public class NoEnemiesGameWonRule : Rule
    {
        public NoEnemiesGameWonRule() : base(triggerOneventId: WellknownGameStateEvents.EntityRemoved)
        {

        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, TriggerDirection direction, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            if (direction != TriggerDirection.After)
            {
                applications = null;
                return false;
            }

            IReadOnlyList<Entity> entities = new List<Entity>(trigger.BasedOnTarget.GetRepresentedEntities(gameStateMutator));
            if (entities.Count != 1)
            {
                applications = null;
                return false;
            }

            if (gameStateMutator.GetNumericQuality(entities[0], WellknownQualities.Faction) != WellknownFactions.Foe)
            {
                applications = null;
                return false;
            }

            // This event must be from when a foe was removed, so check to see if any foes remain
            foreach (Entity curEntity in gameStateMutator.GetAllEntities())
            {
                // If anyone is a foe, there must still be foes remaining
                if (gameStateMutator.GetNumericQuality(curEntity, WellknownQualities.Faction) == WellknownFactions.Foe)
                {
                    applications = null;
                    return false;
                }
            }

            applications = new List<GameStateChange>();
            applications.Add(new SetCampaignState(WellknownCampaignStates.EncounterResolved));
            return true;
        }
    }
}