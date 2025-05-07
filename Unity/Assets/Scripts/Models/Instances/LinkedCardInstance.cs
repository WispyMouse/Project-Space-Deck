namespace SpaceDeck.Models.Instances
{
    using System;
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
        public readonly IReadOnlyList<ExecutionQuestion> Questions;

        public LinkedCardInstance(CardPrototype prototype, IElementProvider elementProvider) : base(prototype.Id, prototype.Qualities.Clone())
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
            this.Questions = this.Prototype?.LinkedTokens?.GetQuestions();
        }

        public override string Describe()
        {
            return this.Prototype.Describe();
        }

        public override EffectDescription GetDescription()
        {
            return new EffectDescription(this.Qualities.GetStringQuality(WellknownQualities.Name, "<unnamed>"), new List<String>() { this.Describe() }, null);
        }

        public override IReadOnlyList<IChangeTarget> GetPossibleTargets(IGameStateMutator mutator)
        {
            return base.GetPossibleTargets(mutator);
        }

        public override IReadOnlyList<ExecutionQuestion> GetQuestions()
        {
            return this.Questions;
        }
    }
}
