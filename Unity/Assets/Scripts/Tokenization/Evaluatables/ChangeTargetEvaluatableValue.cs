namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;

    public abstract class ChangeTargetEvaluatableValue : IEvaluatableValue<IChangeTarget>
    {
        public virtual IReadOnlyList<ExecutionQuestion> GetQuestions(LinkedToken linkedToken)
        {
            return new List<ExecutionQuestion>()
            {
                new EffectTargetExecutionQuestion(linkedToken)
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