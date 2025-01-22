namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections.Generic;

    public class PlayedCardProvider : CardInstanceProvider
    {
        public static readonly PlayedCardProvider Instance = new PlayedCardProvider();

        private PlayedCardProvider()
        {

        }

        public override CardInstance GetProvidedCard(ScriptingExecutionContext answeringContext)
        {
            if (answeringContext?.PlayingCard == null)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.ProviderEvaluation,
                    $"Cannot provide card, as no card was set for the PlayingCard.");
            }
            return answeringContext?.PlayingCard;
        }

        public override CardInstance GetProvidedCard(IGameStateMutator mutator)
        {
            Logging.DebugLog(WellknownLoggingLevels.Error,
                WellknownLoggingCategories.ProviderEvaluation,
                $"Cannot provide card without scripting execution context.");
            return null;
        }

        public override string Describe()
        {
            return string.Empty;
        }
    }
}