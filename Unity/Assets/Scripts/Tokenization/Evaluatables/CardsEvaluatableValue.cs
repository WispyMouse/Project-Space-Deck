namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Evaluatables.Questions;

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
            throw new System.NotImplementedException();
        }

        public bool TryEvaluate(IGameStateMutator mutator, out IReadOnlyList<CardInstance> value)
        {
            throw new System.NotImplementedException();
        }
    }
}