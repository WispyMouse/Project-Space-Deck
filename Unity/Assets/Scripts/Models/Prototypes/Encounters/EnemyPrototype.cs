namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class EnemyPrototype : IHaveQualities
    {
        public LowercaseString Id;
        public LowercaseStringSet Tags;
        public string Name;
        public QualitiesHolder Qualities { get; }

        public EnemyPrototype(LowercaseString id, LowercaseStringSet tags, QualitiesHolder qualities)
        {
            this.Id = id;
            this.Tags = tags;
            this.Qualities = qualities;
        }

    }
}