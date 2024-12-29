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
        public readonly string Name;

        public StatusEffectPrototype(LowercaseString id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public StatusEffectPrototype(LowercaseString id, string name, List<Reactor> reactors) : this(id, name)
        {
            this.Reactors = reactors;
        }
    }
}