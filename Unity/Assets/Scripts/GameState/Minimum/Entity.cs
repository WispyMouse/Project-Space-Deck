namespace SpaceDeck.GameState.Minimum
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents one hollistic object in the game.
    /// The player has an Entity that is always present.
    /// Enemies are generally only Entities that are
    /// stored while in an Encounter.
    /// </summary>
    public class Entity : IChangeTarget
    {
        private readonly Dictionary<string, decimal> Qualities = new Dictionary<string, decimal>();

        /// <summary>
        /// Holds a list to answer the question of <see cref="GetRepresentedEntities"/>.
        /// Since this object only represents itself, it can reuse the same list persistently.
        /// </summary>
        private readonly List<Entity> selfList = new List<Entity>();

        public bool IsAlive = true;

        public Entity()
        {
            this.selfList = new List<Entity>() { this };
        }

        public decimal GetQuality(string index, decimal defaultValue = 0)
        {
            if (this.Qualities.TryGetValue(index, out decimal qualityValue))
            {
                return qualityValue;
            }

            this.Qualities.Add(index, defaultValue);
            return defaultValue;
        }

        public IEnumerable<Entity> GetRepresentedEntities()
        {
            return this.selfList;
        }

        public void SetQuality(string index, decimal newValue)
        {
            if (this.Qualities.ContainsKey(index))
            {
                this.Qualities[index] = newValue;
            }
            else
            {
                this.Qualities.Add(index, newValue);
            }
        }
    }
}