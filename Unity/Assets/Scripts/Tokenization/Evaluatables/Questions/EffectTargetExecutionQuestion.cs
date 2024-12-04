namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System.Collections.Generic;

    /// <summary>
    /// Describes that an effect needs a target to be resolved.
    /// </summary>
    public class EffectTargetExecutionQuestion : ExecutionQuestion<EffectTargetExecutionAnswer>
    {
        public readonly IReadOnlyList<IChangeTarget> Options;

        public EffectTargetExecutionQuestion(LinkedToken linkedToken, IReadOnlyList<IChangeTarget> options) : base(linkedToken)
        {
            this.Options = options;
        }

        public override EffectTargetExecutionAnswer GetTypedAnswerByIndex(int index)
        {
            return new EffectTargetExecutionAnswer(this, Options[index]);
        }
    }
    
    /// <summary>
    /// Describes an answer to <see cref="EffectTargetExecutionQuestion"/>.
    /// </summary>
    public class EffectTargetExecutionAnswer : ExecutionAnswer<EffectTargetExecutionQuestion>
    {
        public IChangeTarget Answer;

        public EffectTargetExecutionAnswer(ExecutionQuestion question, IChangeTarget answer) : base(question)
        {
            this.Answer = answer;
        }
    }
}
