namespace SpaceDeck.Tests.EditMode.Tokenization
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
    using SpaceDeck.GameState.Rules;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    /// <summary>
    /// This class holds tests that were made as part of a
    /// test-driven development process of the Execution Library.
    /// 
    /// Its function serves both as a test, and a working diary
    /// of the features being considered and implemented.
    /// </summary>
    public class RulesTests
    {
        private class TestSpecificTargetAnswerer : AnswererBase
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

        private class ExecuteOnTriggerRule : Rule
        {
            private Action<IGameStateMutator> ToExecute;

            public ExecuteOnTriggerRule(LowercaseString trigger, Action<IGameStateMutator> toExecute) : base(trigger)
            {
                this.ToExecute = toExecute;
            }

            public override bool TryApplyRule(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
            {
                Assert.IsTrue(this.TriggerOnEventIds.Contains(trigger.EventId), "Should only execute rule upon the correct rule event id triggering.");

                applications = new List<GameStateChange>() { new ActionExecutor(this.ToExecute) };
                return true;
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Clear all Scripting Tokens
            ScriptingCommandReference.Clear();
            RuleReference.ClearRules();
        }

        /// <summary>
        /// Creates a rule that knocks-out a character if their health is zero or below.
        /// Then sets a character's health to zero using a GameStateDelta.
        /// </summary>
        [Test]
        public void Rule_ZeroHealth_Enforced()
        {
            const int healthAmount = 100;

            // ARRANGE
            var damageScriptingCommand = new DamageScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(damageScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());
            var targetScriptingCommand = new TargetScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(targetScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new FoeTargetEvaluatableParser());
            ZeroHealthRule zeroLifeIsDeathRule = new ZeroHealthRule();
            RuleReference.RegisterRule(zeroLifeIsDeathRule);

            GameState gameState = new GameState();
            Entity targetingEntity = new Entity();
            targetingEntity.SetQuality(WellknownQualities.Health, healthAmount);
            gameState.CurrentEncounterState.EncounterEnemies.Add(targetingEntity);

            // ACT
            string damageArgumentTokenTextString = $"[TARGET:FOE][{damageScriptingCommand.Identifier}:{healthAmount}]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(damageArgumentTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");
            ExecutionAnswerSet answers = new ExecutionAnswerSet(new EffectTargetExecutionAnswer(linkedTokenSet.GetQuestions()[0], targetingEntity));

            // ASSERT
            Assert.True(GameStateDeltaMaker.TryCreateDelta(linkedTokenSet, answers, gameState, out GameStateDelta generatedDelta), "Should be able to create delta after providing answers.");
            GameStateDeltaApplier.ApplyGameStateDelta(gameState, generatedDelta);
            Assert.False(gameState.EntityIsAlive(targetingEntity), "After losing all health, this entity should be not-alive.");
            Assert.False(gameState.CurrentEncounterState.EncounterEnemies.Contains(targetingEntity), "Dead entities should be removed from the entity list.");
        }

        /// <summary>
        /// Creates a rule that sets a variable when an encounter starts.
        /// Validates that the variable is set.
        /// </summary>
        [Test]
        public void EncounterStart_TriggersRule()
        {
            // ARRANGE
            bool ruleTriggered = false;
            ExecuteOnTriggerRule triggeredRule = new ExecuteOnTriggerRule(WellknownGameStateEvents.EncounterStart, (IGameStateMutator mutator) => ruleTriggered = true);
            RuleReference.RegisterRule(triggeredRule);
            GameState gameState = new GameState();

            // ACT
            gameState.StartEncounter(new EncounterState());
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.True(ruleTriggered, "Rule should be triggered when the encounter starts, changing the variable to true.");
        }
    }
}