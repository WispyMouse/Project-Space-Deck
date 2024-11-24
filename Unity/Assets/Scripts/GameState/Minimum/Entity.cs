namespace SpaceDeck.GameState.Minimum
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents one hollistic object in the game.
    /// The player has an Entity that is always present.
    /// Enemies are generally only Entities that are
    /// stored while in an Encounter.
    /// </summary>
    public class Entity
    {
        private readonly Dictionary<string, decimal> Qualities = new Dictionary<string, decimal>();

        public decimal GetQuality(string index, decimal defaultValue = 0)
        {
            if (this.Qualities.TryGetValue(index, out decimal qualityValue))
            {
                return qualityValue;
            }

            this.Qualities.Add(index, defaultValue);
            return defaultValue;
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