namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;

    public class AppliedStatusEffect : IHaveQualities
    {
        public readonly LowercaseString Id;
        public readonly int StatusEffectPriorityOrder = 0;

        public QualitiesHolder Qualities => this._Qualities;
        private readonly QualitiesHolder _Qualities = new QualitiesHolder();

        public readonly HashSet<LowercaseString> TriggerOnEventIds = new HashSet<LowercaseString>();

        public AppliedStatusEffect(LowercaseString id) : this(id, Array.Empty<LowercaseString>())
        {
            this.Id = id;
        }

        public AppliedStatusEffect(LowercaseString id, IEnumerable<LowercaseString> triggerOnEventIds)
        {
            this.Id = id;
            this.TriggerOnEventIds = new HashSet<LowercaseString>(triggerOnEventIds);
        }

        public virtual bool TryApplyStatusEffect(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            applications = null;
            return false;
        }
    }
}