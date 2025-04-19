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
            this.ElementalGain = new Dictionary<Element, int>();

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

        public override string Describe()
        {
            return this.Prototype.Describe();
        }

        public override EffectDescription GetDescription()
        {
            return new EffectDescription(this.Name, new List<String>() { this.Describe() }, null);
        }

        public override IReadOnlyList<IChangeTarget> GetPossibleTargets(IGameStateMutator mutator)
        {
            return base.GetPossibleTargets(mutator);
        }

        public override IReadOnlyList<ExecutionQuestion> GetQuestions()
        {
            if (this.Prototype?.LinkedTokens == null)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.LinkingFailure, $"Asked entry without a linked token set for questions. This card needs to be linked.");
                return null;
            }

            return this.Prototype.LinkedTokens.Value.GetQuestions();
        }
    }
}
