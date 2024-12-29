namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;

    public class CardsEvaluatableValue : ICardsEvaluatableValue
    {
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