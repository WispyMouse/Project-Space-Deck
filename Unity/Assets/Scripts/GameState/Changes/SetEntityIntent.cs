namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;

    public class SetEntityIntent : GameStateChange
    {
        public readonly Intent SetToIntent;

        public SetEntityIntent(IChangeTarget target, Intent to) : base(target)
        {
            this.SetToIntent = to;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            foreach (Entity curEntity in new List<Entity>(this.Target.GetRepresentedEntities(toApplyTo)))
            {
                string nextIntentDescription = "<none>";
                if (this.SetToIntent != null)
                {
                    nextIntentDescription = this.SetToIntent.Describe();

                    if (string.IsNullOrEmpty(nextIntentDescription))
                    {
                        nextIntentDescription = "<set with no description>";
                    }
                }

                Logging.DebugLog(WellknownLoggingLevels.DebugVerbose, WellknownLoggingCategories.IntentSet, $"Entity {toApplyTo.GetStringQuality(curEntity, WellknownQualities.Name)} sets intent: '{nextIntentDescription}'");

                curEntity.CurrentIntent = this.SetToIntent;
            }
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}