namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a card prototype in the game.
    /// 
    /// The Models.Instances library holds individual instances of the prototype.
    /// </summary>

    public class StatusEffectPrototype
    {
        public readonly LowercaseString Id;
        public readonly List<Reactor> Reactors = new List<Reactor>();

        public StatusEffectPrototype(LowercaseString id)
        {
            this.Id = id;
        }

        public StatusEffectPrototype(LowercaseString id, List<Reactor> reactors) : this(id)
        {
            this.Reactors = reactors;
        }
    }
}