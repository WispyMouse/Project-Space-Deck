namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    public interface ICardsEvaluatableValue : IEvaluatableValue<IReadOnlyList<CardInstance>>
    {
        IReadOnlyList<CardInstance> GetPool(IGameStateMutator mutator);
    }
}