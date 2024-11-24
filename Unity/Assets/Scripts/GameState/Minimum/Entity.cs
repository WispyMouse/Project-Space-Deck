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
        private readonly Dictionary<string, int> Qualities = new Dictionary<string, int>();

        public int GetQuality(string index, int defaultValue = 0)
        {
            if (this.Qualities.TryGetValue(index, out int qualityValue))
            {
                return qualityValue;
            }

            this.Qualities.Add(index, defaultValue);
            return defaultValue;
        }
    }
}