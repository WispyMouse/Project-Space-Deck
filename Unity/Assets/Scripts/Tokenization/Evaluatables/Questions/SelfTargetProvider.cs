namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Evaluatables.Questions;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.Utility.Logging;

    public class SelfTargetProvider : ChangeTargetProvider
    {
        public static readonly SelfTargetProvider Instance = new SelfTargetProvider();

        private SelfTargetProvider()
        {

        }

        public override IReadOnlyList<IChangeTarget> GetProvidedTargets(QuestionAnsweringContext answeringContext)
        {
            if (answeringContext.User != null)
            {
                return new List<IChangeTarget>()
                {
                    answeringContext.User
                };
            }

            Logging.DebugLog(WellknownLoggingLevels.Error,
                WellknownLoggingCategories.ProviderEvaluation,
                $"Cannot use {nameof(SelfTargetProvider)} without the answeringContext having a User.");
            return null;
        }

        public override IReadOnlyList<IChangeTarget> GetProvidedTargets(IGameStateMutator mutator)
        {
            Logging.DebugLog(WellknownLoggingLevels.Error,
                WellknownLoggingCategories.ProviderEvaluation,
                $"Cannot use {nameof(SelfTargetProvider)} using a mutator, as there is no known User.");
            return null;
        }

        public override string Describe()
        {
            return "Foe";
        }
    }

    public class SelfTargetEvaluatableParser : EvaluatableParser<ChangeTargetEvaluatableValue, IChangeTarget>
    {
        private static readonly LowercaseString FoeText = new LowercaseString("SELF");

        public override bool TryParse(LowercaseString argument, out IEvaluatableValue<IChangeTarget> parsedValue)
        {
            if (argument.Equals(FoeText))
            {
                parsedValue = new ChangeTargetEvaluatableValue(SelfTargetProvider.Instance);
                return true;
            }

            parsedValue = null;
            return false;
        }
    }
}