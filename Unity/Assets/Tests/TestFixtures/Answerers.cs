namespace SpaceDeck.Tests.EditMode.TestFixtures
{
    using NUnit.Framework;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Events;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.GameState.Changes.Targets;
    using SpaceDeck.Tokenization.ScriptingCommands;
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.GameState.Context;
    using SpaceDeck.Tokenization.Evaluatables.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

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
                IChangeTarget target = targetQuestion.Options.GetProvidedTargets(answeringContext)[this.Index];
                answerReceiver.Invoke(new EffectTargetExecutionAnswer(question, target));
            }
            else
            {
                Assert.Fail($"This answerer does not have the tools to handle this question.");
            }
        }
    }
}