namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    public class CardInstance : IDescribable, IHaveQualities
    {
        public readonly LowercaseString Id;
        public readonly string Name;

        public QualitiesHolder Qualities => new QualitiesHolder();

        public virtual string Describe()
        {
            throw new System.NotImplementedException();
        }
    }
}