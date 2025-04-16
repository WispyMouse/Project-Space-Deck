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
    using SpaceDeck.Utility.Wellknown;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class EntityPicksIntentEncounterStartRule : Rule
    {
        public EntityPicksIntentEncounterStartRule() : base(WellknownGameStateEvents.EncounterStart, -1)
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
                Intent nextIntent = curEntity.GetNextAttack();

                string nextIntentDescription = "<none>";
                if (nextIntent != null)
                {
                    nextIntentDescription = nextIntent.Describe();

                    if (string.IsNullOrEmpty(nextIntentDescription))
                    {
                        nextIntentDescription = "<set with no description>";
                    }
                }

                Logging.DebugLog(WellknownLoggingLevels.DebugVerbose, WellknownLoggingCategories.IntentSet, $"Entity {curEntity.Qualities.GetStringQuality(WellknownQualities.Name)} sets intent: {nextIntentDescription}");
                if (nextIntent != null)
                {
                    applications.Add(new SetEntityIntent(curEntity, nextIntent));
                }
            }

            return true;
        }
    }
}