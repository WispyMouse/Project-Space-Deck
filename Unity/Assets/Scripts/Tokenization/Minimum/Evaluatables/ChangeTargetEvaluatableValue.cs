using SpaceDeck.GameState.Minimum;

namespace SpaceDeck.Tokenization.Minimum.Evaluatables
{
    public abstract class ChangeTargetEvaluatableValue : IEvaluatableValue<IChangeTarget>
    {
        public abstract bool TryEvaluate(ExecutionContext context, out IChangeTarget value);
    }
}