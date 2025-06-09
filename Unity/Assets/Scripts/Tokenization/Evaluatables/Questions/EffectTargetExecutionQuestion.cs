namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;
    using System.Collections.Generic;

    /// <summary>
    /// Describes that an effect needs a target to be resolved.
    /// </summary>
    public class EffectTargetExecutionQuestion : LinkedExecutionQuestion<EffectTargetExecutionAnswer>
    {
        public readonly ChangeTargetProvider Options;

        public EffectTargetExecutionQuestion(LinkedToken linkedToken, ChangeTargetProvider options) : base(linkedToken)
        {
            if (options == null)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.Question, $"{nameof(EffectTargetExecutionQuestion)} created with null options.");
            }

            this.Options = options;
        }

        public override bool TryGetDefaultTypedAnswer(QuestionAnsweringContext answeringContext, out EffectTargetExecutionAnswer answer)
        {
            IReadOnlyList<IChangeTarget> targets = this.Options.GetProvidedTargets(answeringContext);
            if (targets != null && targets.Count == 1)
            {
                answer = new EffectTargetExecutionAnswer(this, targets[0]);
                return true;
            }

            answer = null;
            return false;
        }

        public override void ApplyDefaultToContext(QuestionAnsweringContext answeringContext)
        {
            IReadOnlyList<IChangeTarget> targets = this.Options.GetProvidedTargets(answeringContext);
            if (targets != null && targets.Count == 1)
            {
                answeringContext.DefaultTarget = targets[0];
            }
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

        public override void ApplyToQuestionAnsweringContext(QuestionAnsweringContext context)
        {
            context.DefaultTarget = this.Answer;
        }
    }
}
