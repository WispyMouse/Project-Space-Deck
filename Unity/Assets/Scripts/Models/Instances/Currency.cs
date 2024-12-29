namespace SpaceDeck.Models.Instances
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    public class Currency
    {
        public readonly LowercaseString Id;
        public readonly string Name;
        public int? SpriteIndex;

        public Currency(LowercaseString id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public string GetNameOrIcon()
        {
            if (this.SpriteIndex.HasValue)
            {
                return $"<sprite index={this.SpriteIndex.Value}>";
            }

            return this.Name;
        }

        public string GetNameAndMaybeIcon()
        {
            if (this.SpriteIndex.HasValue)
            {
                return $"<sprite index={this.SpriteIndex.Value}>{this.Name}";
            }

            return this.Name;
        }
    }
}