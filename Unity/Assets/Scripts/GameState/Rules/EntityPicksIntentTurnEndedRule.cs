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
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class EntityPicksIntentTurnEndedRule : Rule
    {
        public EntityPicksIntentTurnEndedRule() : base(WellknownGameStateEvents.FactionTurnEnded)
        {
        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, TriggerDirection direction, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            if (direction != TriggerDirection.Before)
            {
                applications = null;
                return false;
            }

            applications = new List<GameStateChange>();

            foreach (Entity curEntity in gameStateMutator.GetAllEntities())
            {
                // Somehow, this doesn't have a faction?
                if (!curEntity.Qualities.TryGetNumericQuality(WellknownQualities.Faction, out decimal curEntityFaction))
                {
                    continue;
                }

                // Only for the faction that is ended the turn
                if (curEntityFaction != gameStateMutator.FactionTurnTakerCalculator.GetCurrentFaction())
                {
                    continue;
                }

                Intent nextIntent = curEntity.GetNextAttack();
                if (nextIntent != null)
                {
                    applications.Add(new SetEntityIntent(curEntity, nextIntent));
                }
            }
            
            return true;
        }
    }
}