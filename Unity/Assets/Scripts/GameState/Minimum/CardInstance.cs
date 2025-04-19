namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;

    public abstract class CardInstance : IDescribable, IEffectIDescribable, IHaveQualities
    {
        public readonly LowercaseString Id;
        public Dictionary<Element, int> ElementalGain;

        public QualitiesHolder Qualities { get; } = new QualitiesHolder();

        public CardInstance(LowercaseString id, QualitiesHolder qualities = null)
        {
            this.Id = id;
            this.Qualities = qualities ?? new QualitiesHolder();
        }

        public abstract string Describe();

        public abstract EffectDescription GetDescription();

        public virtual IReadOnlyList<IChangeTarget> GetPossibleTargets(IGameStateMutator mutator)
        {
            return Array.Empty<IChangeTarget>();
        }

        public virtual IReadOnlyList<ExecutionQuestion> GetQuestions()
        {
            return Array.Empty<ExecutionQuestion>();
        }
    }
}