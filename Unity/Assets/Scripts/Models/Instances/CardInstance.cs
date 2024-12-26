namespace SpaceDeck.Models.Instances
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Models.Prototypes;

    /// <summary>
    /// Represents a specific instance of a card.
    /// 
    /// The player's deck will contain CardInstances, rather than CardPrototypes.
    /// </summary>
    public class CardInstance
    {
        public readonly CardPrototype Prototype;

        public CardInstance(CardPrototype prototype)
        {
            this.Prototype = prototype;
        }
    }
}
