namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class EnemyPrototype : IHaveQualities
    {
        public readonly LowercaseString Id;
        public readonly LowercaseStringSet Tags;
        public readonly string Name;
        public QualitiesHolder Qualities { get; }
        public readonly Dictionary<LowercaseString, EnemyAttack> AttackScripts = new Dictionary<LowercaseString, EnemyAttack>();

        public EnemyPrototype(LowercaseString id, LowercaseStringSet tags, QualitiesHolder qualities, Dictionary<LowercaseString, EnemyAttack> attacks)
        {
            this.Id = id;
            this.Tags = tags;
            this.Qualities = qualities;
            this.AttackScripts = attacks;
        }

    }
}