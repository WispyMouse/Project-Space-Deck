namespace SpaceDeck.Tokenization.Minimum.Evaluatables
{
    using SpaceDeck.GameState.Minimum;

    public class DefaultTargetEvaluatableValue : ChangeTargetEvaluatableValue
    {
        public static readonly DefaultTargetEvaluatableValue Instance = new DefaultTargetEvaluatableValue();

        private DefaultTargetEvaluatableValue()
        {

        }

        public override bool TryEvaluate(ExecutionContext context, out IChangeTarget value)
        {
            value = context.CurrentDefaultTarget;
            return value != null;
        }
    }
}