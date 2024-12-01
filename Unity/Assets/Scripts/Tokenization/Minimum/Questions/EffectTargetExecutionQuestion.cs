namespace SpaceDeck.Tokenization.Minimum.Questions
{
    using SpaceDeck.GameState.Minimum;

    /// <summary>
    /// Describes that an effect needs a target to be resolved.
    /// </summary>
    public class EffectTargetExecutionQuestion : ExecutionQuestion
    {

    }
    
    /// <summary>
    /// Describes an answer to <see cref="EffectTargetExecutionQuestion"/>.
    /// </summary>
    public class EffectTargetExecutionAnswer : ExecutionAnswer<EffectTargetExecutionQuestion>
    {
        public IChangeTarget Answer;

        public EffectTargetExecutionAnswer(IChangeTarget answer)
        {
            this.Answer = answer;
        }
    }
}
