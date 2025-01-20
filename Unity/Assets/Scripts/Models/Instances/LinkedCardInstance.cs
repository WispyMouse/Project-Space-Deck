namespace SpaceDeck.Models.Instances
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    /// <summary>
    /// Represents a specific instance of a card.
    /// 
    /// The player's deck will contain CardInstances, rather than CardPrototypes.
    /// </summary>
    public class LinkedCardInstance : CardInstance
    {
        public readonly CardPrototype Prototype;

        public LinkedCardInstance(CardPrototype prototype, IElementProvider elementProvider)
        {
            this.Prototype = prototype;

            foreach (LowercaseString elementId in (prototype.ElementalGain != null ? (IEnumerable<LowercaseString>)(prototype.ElementalGain.Keys) : Array.Empty<LowercaseString>()))
            {
                if (!elementProvider.TryGetElement(elementId, out var element))
                {
                    Logging.DebugLog(WellknownLoggingLevels.Error,
                        WellknownLoggingCategories.LinkConstructor,
                        $"Could not find element '{elementId}'.");
                }

                this.ElementalGain.Add(element, prototype.ElementalGain[elementId]);
            }
        }
    }
}
