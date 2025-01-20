namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;

    public class CardInstance : IDescribable, IEffectIDescribable, IHaveQualities
    {
        public readonly LowercaseString Id;
        public readonly string Name;
        public Dictionary<Element, int> ElementalGain;

        public QualitiesHolder Qualities => new QualitiesHolder();

        public CardInstance()
        {

        }

        public virtual string Describe()
        {
            throw new System.NotImplementedException();
        }

        public virtual EffectDescription GetDescription()
        {
            throw new System.NotImplementedException();
        }

        public virtual IReadOnlyList<IChangeTarget> GetPossibleTargets(IGameStateMutator mutator)
        {
            return Array.Empty<IChangeTarget>();
        }
    }
}