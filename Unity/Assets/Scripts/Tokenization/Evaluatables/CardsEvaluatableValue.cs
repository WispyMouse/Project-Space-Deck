namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Evaluatables.Questions;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;

    public class CardsEvaluatableValue : ICardsEvaluatableValue
    {
        public readonly CardInstanceProvider CardInstanceProvider;
        public readonly CardInstancesProvider CardInstancesProvider;

        public CardsEvaluatableValue(CardInstanceProvider provider)
        {
            this.CardInstanceProvider = provider;
        }

        public CardsEvaluatableValue(CardInstancesProvider provider)
        {
            this.CardInstancesProvider = provider;
        }

        public string Describe()
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<CardInstance> GetPool(IGameStateMutator mutator)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<ExecutionQuestion> GetQuestions(LinkedToken linkedToken)
        {
            throw new System.NotImplementedException();
        }

        public bool TryEvaluate(ScriptingExecutionContext context, out IReadOnlyList<CardInstance> value)
        {
            if (this.CardInstancesProvider != null)
            {
                value = this.CardInstancesProvider.GetProvidedCards(context);

                // It's valid for this list to be empty, or very full
                // Just as long as it's not null
                if (value == null)
                {
                    value = new List<CardInstance>();
                }
                return true;
            }
            else if (this.CardInstanceProvider != null)
            {
                CardInstance oneCard = this.CardInstanceProvider.GetProvidedCard(context);

                if (oneCard == null)
                {
                    Logging.DebugLog(WellknownLoggingLevels.Error,
                        WellknownLoggingCategories.EvaluatableEvaluation,
                        $"Failed to evaluate a single card from the card instance provider. '{this.CardInstanceProvider.Describe()}'");
                    value = null;
                    return false;
                }

                value = new List<CardInstance>() { oneCard };
                return true;
            }

            value = null;
            return false;
        }

        public bool TryEvaluate(IGameStateMutator mutator, out IReadOnlyList<CardInstance> value)
        {
            if (this.CardInstancesProvider != null)
            {
                value = this.CardInstancesProvider.GetProvidedCards(mutator);

                // It's valid for this list to be empty, or very full
                // Just as long as it's not null
                if (value == null)
                {
                    value = new List<CardInstance>();
                }
                return true;
            }
            else if (this.CardInstanceProvider != null)
            {
                CardInstance oneCard = this.CardInstanceProvider.GetProvidedCard(mutator);

                if (oneCard == null)
                {
                    Logging.DebugLog(WellknownLoggingLevels.Error,
                        WellknownLoggingCategories.EvaluatableEvaluation,
                        $"Failed to evaluate a single card from the card instance provider. '{this.CardInstanceProvider.Describe()}'");
                    value = null;
                    return false;
                }

                value = new List<CardInstance>() { oneCard };
                return true;
            }

            value = null;
            return false;
        }
    }
}