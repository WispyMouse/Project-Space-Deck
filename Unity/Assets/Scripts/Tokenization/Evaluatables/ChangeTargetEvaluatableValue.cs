namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Evaluatables.Questions;

    public abstract class ChangeTargetEvaluatableValue : IEvaluatableValue<IChangeTarget>
    {
        public readonly List<IChangeTarget> Options = new List<IChangeTarget>();

        public virtual IReadOnlyList<ExecutionQuestion> GetQuestions(LinkedToken linkedToken)
        {
            return new List<ExecutionQuestion>()
            {
                new EffectTargetExecutionQuestion(linkedToken, this.Options)
            };
        }

        public bool TryEvaluate(ExecutionAnswerSet answers, out IChangeTarget value)
        {
            if (!answers.TryGetTypedAnswerForQuestionType(out EffectTargetExecutionQuestion question, out EffectTargetExecutionAnswer answer))
            {
                value = null;
                return false;
            }

            value = answer.Answer;
            return true;
        }
    }
}