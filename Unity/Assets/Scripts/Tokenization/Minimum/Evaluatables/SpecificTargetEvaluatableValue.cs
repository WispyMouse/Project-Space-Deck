using SpaceDeck.GameState.Minimum;

namespace SpaceDeck.Tokenization.Minimum.Evaluatables
{

    public class SpecificTargetEvaluatableValue : ChangeTargetEvaluatableValue
    {
        public readonly IChangeTarget Target;

        public SpecificTargetEvaluatableValue(IChangeTarget target)
        {
            this.Target = target;
        }

        public override bool TryEvaluate(ExecutionContext context, out IChangeTarget value)
        {
            value = this.Target;
            return this.Target != null;
        }
    }
}