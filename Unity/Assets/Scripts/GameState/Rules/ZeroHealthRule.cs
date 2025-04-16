namespace SpaceDeck.GameState.Rules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class ZeroHealthRule : Rule
    {
        public ZeroHealthRule() : base(WellknownGameStateEvents.RuleApplication)
        {

        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, TriggerDirection direction, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            if (direction != TriggerDirection.After)
            {
                applications = null;
                return false;
            }

            applications = new List<GameStateChange>();

            foreach (Entity curEntity in gameStateMutator.GetAllEntities())
            {
                if (gameStateMutator.EntityIsAlive(curEntity) 
                    && gameStateMutator.GetNumericQuality(curEntity, WellknownQualities.Health, 0) <= 0
                    && gameStateMutator.GetNumericQuality(curEntity, WellknownQualities.DeathFlag, 0) == 0)
                {
                    // Immediately mark this entity for death; do this in both an application level and an immediate level
                    // By setting it directly in this mutator, it won't trigger again before the resolution
                    // but still call out a set numeric quality flag operation so it gets passed to the base game state
                    gameStateMutator.SetNumericQuality(curEntity, WellknownQualities.DeathFlag, 1);
                    applications.Add(new SetNumericQuality(curEntity, curEntity, WellknownQualities.DeathFlag, 1));
                    applications.Add(new RemoveEntity(curEntity));
                }
            }

            return applications.Count > 0;
        }
    }
}