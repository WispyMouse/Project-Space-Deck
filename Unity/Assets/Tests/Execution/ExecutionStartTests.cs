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

    /// <summary>
    /// This class holds tests that were made as part of a
    /// test-driven development process of the Execution Library.
    /// 
    /// Its function serves both as a test, and a working diary
    /// of the features being considered and implemented.
    /// </summary>
    public class ExecutionStartTests
    {
        private class LoggingGameStateChange : GameStateChange
        {
            public readonly string ToLog;

            public LoggingGameStateChange(string toLog) : base(NobodyTarget.Instance)
            {
                this.ToLog = toLog;
            }

            public override void ApplyToGameState(ref GameState toApplyTo)
            {
                Debug.Log(this.ToLog);
            }
        }

        private class ZeroArgumentDebugLogScriptingCommand : ScriptingCommand
        {
            public const string HelloString = "HELLO!";
            public static readonly LowercaseString IdentifierString = new LowercaseString("ZEROARGUMENTDEBUG");
            public override LowercaseString Identifier => IdentifierString;

            public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
            {
                linkedToken = new ZeroArgumentDebugLogLinkedToken(parsedToken);
                return true;
            }
        }

        private class ZeroArgumentDebugLogLinkedToken : LinkedToken<ZeroArgumentDebugLogScriptingCommand>
        {
            public ZeroArgumentDebugLogLinkedToken(ParsedToken parsedToken) : base(parsedToken)
            {
            }

            public override bool TryGetChanges(GameState stateToApplyTo, ExecutionAnswerSet answers, out List<GameStateChange> changes)
            {
                changes = new List<GameStateChange>();
                changes.Add(new LoggingGameStateChange(ZeroArgumentDebugLogScriptingCommand.HelloString));
                return true;
            }
        }

        private class OneArgumentDebugLogScriptingCommand : ScriptingCommand
        {
            public static readonly LowercaseString IdentifierString = new LowercaseString("ONEARGUMENTDEBUG");
            public override LowercaseString Identifier => IdentifierString;

            public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
            {
                linkedToken = new OneArgumentDebugLogLinkedToken(parsedToken);
                return true;
            }
        }

        private class OneArgumentDebugLogLinkedToken : LinkedToken<OneArgumentDebugLogScriptingCommand>
        {
            public OneArgumentDebugLogLinkedToken(ParsedToken parsedToken) : base(parsedToken)
            {
            }

            public override bool TryGetChanges(GameState stateToApplyTo, ExecutionAnswerSet answers, out List<GameStateChange> changes)
            {
                if (this.Arguments == null || this.Arguments.Count == 0)
                {
                    changes = null;
                    return false;
                }

                changes = new List<GameStateChange>();
                changes.Add(new LoggingGameStateChange(this.Arguments[0].ToString()));
                return true;
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Clear all Scripting Tokens
            ScriptingCommandReference.Clear();
        }

        /// <summary>
        /// This creates an effect that will result in
        /// logging out a simple message in the Debug logs.
        /// </summary>
        [Test]
        public void DebugLog_NoArguments_AsExpected()
        {
            ScriptingCommandReference.RegisterScriptingCommand(new ZeroArgumentDebugLogScriptingCommand());

            string zeroArgumentDebugLogTokenTextString = $"[{ZeroArgumentDebugLogScriptingCommand.IdentifierString}]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(zeroArgumentDebugLogTokenTextString, out TokenText zeroArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(zeroArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");

            GameState gameState = new GameState();

            ContextualizedTokenList contextualizedTokens = new ContextualizedTokenList(linkedTokenSet);
            Assert.True(GameStateDeltaMaker.TryCreateDelta(contextualizedTokens, gameState, out GameStateDelta generatedDelta), "Should be able to create a game state delta from provided context.");

            Assert.AreEqual(1, generatedDelta.Changes.Count, "Expecting one change.");
            Assert.IsTrue(generatedDelta.Changes[0] is LoggingGameStateChange, "Expecting token to be a logging token.");
            LoggingGameStateChange log = generatedDelta.Changes[0] as LoggingGameStateChange;
            Assert.AreEqual(ZeroArgumentDebugLogScriptingCommand.HelloString, log.ToLog, "Expecting debug log to be as designated.");
        }

        /// <summary>
        /// This creates an effect that will result in
        /// logging out a simple message in the Debug logs.
        /// </summary>
        [Test]
        public void DebugLog_OneArgument_AsExpected()
        {
            ScriptingCommandReference.RegisterScriptingCommand(new OneArgumentDebugLogScriptingCommand());

            string oneArgumentDebugLogTokenTextString = $"[{OneArgumentDebugLogScriptingCommand.IdentifierString}:111]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(oneArgumentDebugLogTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");

            GameState gameState = new GameState();

            ContextualizedTokenList contextualizedTokens = new ContextualizedTokenList(linkedTokenSet);
            Assert.True(GameStateDeltaMaker.TryCreateDelta(contextualizedTokens, gameState, out GameStateDelta generatedDelta), "Should be able to create a game state delta from provided context.");
            Assert.AreEqual(1, generatedDelta.Changes.Count, "Expecting one change.");
            Assert.IsTrue(generatedDelta.Changes[0] is LoggingGameStateChange, "Expecting token to be a logging token.");
            LoggingGameStateChange log = generatedDelta.Changes[0] as LoggingGameStateChange;
            Assert.AreEqual("111", log.ToLog, "Expecting debug log to be as designated.");
        }

        /// <summary>
        /// This test creates a command involving dealing damage.
        /// It doesn't specify a target beforehand, and so it should fail to evaluate.
        /// </summary>
        [Test]
        public void Damage_RequiresTarget_FailWithoutTarget()
        {
            // ARRANGE
            var damageScriptingCommand = new DamageScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(damageScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());

            GameState gameState = new GameState();
            Entity targetingEntity = new Entity();
            targetingEntity.SetQuality("health", 100);
            gameState.PersistentEntities.Add(targetingEntity);

            // ACT
            string damageArgumentTokenTextString = $"[{damageScriptingCommand.Identifier}:1]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(damageArgumentTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");
            ContextualizedTokenList contextualizedTokens = new ContextualizedTokenList(linkedTokenSet);

            // ASSERT
            // DAMAGE requires a target, and without a target this shouldn't be able to evaluate
            Assert.False(GameStateDeltaMaker.TryCreateDelta(contextualizedTokens, gameState, out GameStateDelta generatedDelta), "Should not be able to create delta without providing context.");
        }

        /// <summary>
        /// This test creates a command involving dealing damage.
        /// The a target is provided programmatically, showing that it can evaluate.
        /// </summary>
        [Test]
        public void Damage_RequiresTarget_SucceedWithTarget()
        {
            // ARRANGE
            var damageScriptingCommand = new DamageScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(damageScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());

            GameState gameState = new GameState();
            Entity targetingEntity = new Entity();
            targetingEntity.SetQuality("health", 100);
            gameState.PersistentEntities.Add(targetingEntity);

            Dictionary<LinkedToken, ExecutionAnswerSet> answers = new Dictionary<LinkedToken, ExecutionAnswerSet>();

            // ACT
            string damageArgumentTokenTextString = $"[{damageScriptingCommand.Identifier}:1]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(damageArgumentTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");
            ContextualizedTokenList contextualizedTokens = new ContextualizedTokenList(linkedTokenSet);

            // ASSERT
            // With the appropriate answers provided, assert this can be performed
            answers.Add(contextualizedTokens.Questions[0].Token, new ExecutionAnswerSet(contextualizedTokens.Questions[0], new EffectTargetExecutionAnswer(targetingEntity)));
            Assert.True(GameStateDeltaMaker.TryCreateDelta(contextualizedTokens, answers, gameState, out GameStateDelta generatedDelta), "Should be able to create delta after providing answers.");
        }

        /// <summary>
        /// This is a test of the system leading up to damage being applied to a character.
        /// Through the flow of parsing, linking, and applying to a game state, we can see if damage can result
        /// in a different game state value being set when the entire operation runs.
        /// 
        /// </summary>
        [Test]
        public void Damage_CausesHealthLoss()
        {
            var damageScriptingCommand = new DamageScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(damageScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());

            string damageArgumentTokenTextString = $"[{damageScriptingCommand.Identifier}:1]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(damageArgumentTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");
            Assert.True(linkedTokenSet.Scopes.Count == 1 && linkedTokenSet.Scopes[0].Tokens.Count == 1 && linkedTokenSet.Scopes[0].Tokens[0] is DamageLinkedToken damageToken, $"Expecting linking to result in a single token of the {nameof(DamageLinkedToken)} type.");

            GameState gameState = new GameState();
            Entity targetingEntity = new Entity();
            targetingEntity.SetQuality("health", 100);
            gameState.PersistentEntities.Add(targetingEntity);

            ContextualizedTokenList contextualizedTokens = new ContextualizedTokenList(linkedTokenSet);
            Dictionary<LinkedToken, ExecutionAnswerSet> answers = new Dictionary<LinkedToken, ExecutionAnswerSet>();
            answers.Add(contextualizedTokens.Tokens.First, new ExecutionAnswerSet(contextualizedTokens.Questions[0], new EffectTargetExecutionAnswer(targetingEntity)));
            Assert.True(GameStateDeltaMaker.TryCreateDelta(contextualizedTokens, answers, gameState, out GameStateDelta generatedDelta), "Should be able to create a game state delta from provided context.");

            Assert.AreEqual(1, generatedDelta.Changes.Count, "Expecting one change.");
            Assert.IsTrue(generatedDelta.Changes[0] is ModifyQuality, "Expecting token to be a quality change token.");
            ModifyQuality modifyQuality = generatedDelta.Changes[0] as ModifyQuality;
            Assert.AreEqual(modifyQuality.ModifyValue, -1, "Expecting damage amount to be (negative) one.");

            GameStateDeltaApplier.ApplyGameStateDelta(ref gameState, generatedDelta, new ExecutionContext() { CurrentDefaultTarget = targetingEntity });
            Assert.AreEqual(99, gameState.PersistentEntities[0].GetQuality("health"), "Expecting health to currently be 1 less than starting, so 99.");
        }
    }
}