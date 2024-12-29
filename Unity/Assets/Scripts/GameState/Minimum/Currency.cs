namespace SpaceDeck.GameState.Minimum
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;

    public class Currency
    {
        public readonly LowercaseString Id;
        public readonly string Name;

        public Currency(LowercaseString id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}