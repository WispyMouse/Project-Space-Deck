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
    public class Entity : IChangeTarget
    {
        private readonly Dictionary<LowercaseString, decimal> NumericQualities = new Dictionary<LowercaseString, decimal>();
        private readonly Dictionary<LowercaseString, string> StringQualities = new Dictionary<LowercaseString, string>();

        /// <summary>
        /// Holds a list to answer the question of <see cref="GetRepresentedEntities"/>.
        /// Since this object only represents itself, it can reuse the same list persistently.
        /// </summary>
        private readonly List<Entity> selfList;

        public Entity()
        {
            this.selfList = new List<Entity>() { this };
        }

        public decimal GetNumericQuality(LowercaseString index, decimal defaultValue = 0)
        {
            if (this.NumericQualities.TryGetValue(index, out decimal qualityValue))
            {
                return qualityValue;
            }

            this.NumericQualities.Add(index, defaultValue);
            return defaultValue;
        }

        public string GetStringQuality(LowercaseString index, string defaultValue = "")
        {
            if (this.StringQualities.TryGetValue(index, out string qualityValue))
            {
                return qualityValue;
            }

            this.StringQualities.Add(index, defaultValue);
            return defaultValue;
        }

        public IEnumerable<Entity> GetRepresentedEntities(IGameStateMutator gameState)
        {
            return this.selfList;
        }

        public void SetNumericQuality(LowercaseString index, decimal newValue)
        {
            if (this.NumericQualities.ContainsKey(index))
            {
                this.NumericQualities[index] = newValue;
            }
            else
            {
                this.NumericQualities.Add(index, newValue);
            }
        }

        public void SetStringQuality(LowercaseString index, string newValue)
        {
            if (this.StringQualities.ContainsKey(index))
            {
                this.StringQualities[index] = newValue;
            }
            else
            {
                this.StringQualities.Add(index, newValue);
            }

        }
    }
}