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

    public class PlayerRemovedGameOverRule : Rule
    {
        public PlayerRemovedGameOverRule() : base(triggerOneventId: WellknownGameStateEvents.EntityRemoved)
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

            if (gameStateMutator.GetNumericQuality(entities[0], WellknownQualities.Faction) != WellknownFactions.Player)
            {
                applications = null;
                return false;
            }

            applications = new List<GameStateChange>();
            applications.Add(new SetCampaignState(WellknownCampaignStates.GameOver));
            return true;
        }
    }
}