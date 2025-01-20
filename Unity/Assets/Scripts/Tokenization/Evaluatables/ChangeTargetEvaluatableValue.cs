namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Evaluatables.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;

    public class ChangeTargetEvaluatableValue : IEvaluatableValue<IChangeTarget>
    {
        public readonly ChangeTargetProvider Provider;

        public ChangeTargetEvaluatableValue(ChangeTargetProvider provider)
        {
            this.Provider = provider;
        }

        public virtual IReadOnlyList<ExecutionQuestion> GetQuestions(LinkedToken linkedToken)
        {
            return new List<ExecutionQuestion>()
            {
                new EffectTargetExecutionQuestion(linkedToken, this.Provider)
            };
        }

        public bool TryEvaluate(ScriptingExecutionContext context, out IChangeTarget value)
        {
            if (!context.Answers.TryGetTypedAnswerForQuestionType(out EffectTargetExecutionQuestion question, out EffectTargetExecutionAnswer answer))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.EvaluatableEvaluation,
                    $"Could not find answer for typed question in answer set.");
                value = null;
                return false;
            }

            value = answer.Answer;
            return true;
        }

        public bool TryEvaluate(IGameStateMutator mutator, out IChangeTarget value)
        {
            IReadOnlyList<IChangeTarget> targets = this.Provider.GetProvidedTargets(mutator);
            if (targets == null || targets.Count == 0)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.EvaluatableEvaluation,
                    $"Failed to evaluate targetable for provider. '{this.Provider.Describe()}'");
                value = null;
                return false;
            }
            value = targets[0];
            return true;
        }

        public virtual string Describe()
        {
            return this.Provider.Describe();
        }
    }
}