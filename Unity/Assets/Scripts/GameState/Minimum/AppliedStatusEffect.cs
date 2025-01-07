namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class AppliedStatusEffect : IHaveQualities, IDescribable, IEffectIDescribable
    {
        public readonly LowercaseString Id;
        public readonly int StatusEffectPriorityOrder = 0;
        public readonly string Name;

        public QualitiesHolder Qualities => this._Qualities;
        private readonly QualitiesHolder _Qualities = new QualitiesHolder();

        public readonly HashSet<LowercaseString> TriggerOnEventIds = new HashSet<LowercaseString>();

        public AppliedStatusEffect(LowercaseString id, string name, IEnumerable<LowercaseString> triggerOnEventIds)
        {
            this.Id = id;
            this.Name = name;
            this.TriggerOnEventIds = new HashSet<LowercaseString>(triggerOnEventIds);
        }

        public AppliedStatusEffect(LowercaseString id, string name) : this(id, name, Array.Empty<LowercaseString>())
        {
        }

        public virtual bool TryApplyStatusEffect(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, TriggerDirection direction, out List<GameStateChange> applications)
        {
            applications = null;
            return false;
        }

        public string Describe()
        {
            throw new NotImplementedException();
        }

        public EffectDescription GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}