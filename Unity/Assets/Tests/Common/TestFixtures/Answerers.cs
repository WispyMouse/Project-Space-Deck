namespace SpaceDeck.Tests.EditMode.Common.TestFixtures
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using SpaceDeck.Tokenization.Evaluatables.Questions;

    public class TestSpecificTargetAnswerer : AnswererBase
    {
        private IChangeTarget Target;

        public TestSpecificTargetAnswerer(IChangeTarget target)
        {
            this.Target = target;
        }

        public override void HandleQuestion(QuestionAnsweringContext answeringContext, ExecutionQuestion question, ProvideQuestionAnswerDelegate answerReceiver)
        {
            answerReceiver.Invoke(new EffectTargetExecutionAnswer(question, this.Target));
        }
    }

    public class IndexChoosingAnswerer : AnswererBase
    {
        private int Index;

        public IndexChoosingAnswerer(int index)
        {
            this.Index = index;
        }

        public override void HandleQuestion(QuestionAnsweringContext answeringContext, ExecutionQuestion question, ProvideQuestionAnswerDelegate answerReceiver)
        {
            if (question is EffectTargetExecutionQuestion targetQuestion)
            {
                IReadOnlyList<IChangeTarget> targets = targetQuestion.Options.GetProvidedTargets(answeringContext);

                if (targets == null)
                {
                    Assert.Fail($"Asked for targets and received a null list. {nameof(targetQuestion.Options.GetProvidedTargets)} may be empty, but should never be null.");
                }

                if (targets.Count < this.Index)
                {
                    Assert.Fail($"This answerer was set to answer '{this.Index}', but the list of available targets contains only '{targets.Count}' targets.");
                }

                IChangeTarget target = targets[this.Index];
                answerReceiver.Invoke(new EffectTargetExecutionAnswer(question, target));
            }
            else
            {
                Assert.Fail($"This answerer does not have the tools to handle this question.");
            }
        }
    }
}