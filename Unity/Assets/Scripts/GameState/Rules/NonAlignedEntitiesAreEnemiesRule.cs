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

    public class NonAlignedEntitiesAreEnemiesRule : Rule
    {
        public NonAlignedEntitiesAreEnemiesRule() : base(WellknownGameStateEvents.EncounterStart, 10)
        {
        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, TriggerDirection direction, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            if (direction != TriggerDirection.Before)
            {
                applications = null;
                return false;
            }

            applications = new List<GameStateChange>() { };
            foreach (Entity curEntity in gameStateMutator.GetAllEntities())
            {
                if (!gameStateMutator.HasNumericQuality(curEntity, WellknownQualities.Faction))
                {
                    applications.Add(new SetNumericQuality(curEntity, curEntity, WellknownQualities.Faction, WellknownFactions.Foe));
                }
            }

            return true;
        }
    }
}