namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    /// <summary>
    /// Represents one hollistic object in the game.
    /// The player has an Entity that is always present.
    /// Enemies are generally only Entities that are
    /// stored while in an Encounter.
    /// </summary>
    public class Entity : IChangeTarget, IHaveQualities
    {
        public QualitiesHolder Qualities => this._Qualities;
        private readonly QualitiesHolder _Qualities = new QualitiesHolder();

        public readonly Dictionary<LowercaseString, AppliedStatusEffect> AppliedStatusEffects = new Dictionary<LowercaseString, AppliedStatusEffect>();

        /// <summary>
        /// Holds a list to answer the question of <see cref="GetRepresentedEntities"/>.
        /// Since this object only represents itself, it can reuse the same list persistently.
        /// </summary>
        private readonly List<Entity> selfList;

        public Intent CurrentIntent { get; set; }

        public Entity()
        {
            this.selfList = new List<Entity>() { this };
        }

        public IEnumerable<Entity> GetRepresentedEntities(IGameStateMutator gameState)
        {
            return this.selfList;
        }
    }
}