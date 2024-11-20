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

    /// <summary>
    /// This class holds tests that were made as part of a
    /// test-driven development process of the Execution Library.
    /// 
    /// Its function serves both as a test, and a working diary
    /// of the features being considered and implemented.
    /// </summary>
    public class ExecutionStartTests
    {
        private class ZeroArgumentDebugLogScriptingCommand : ScriptingCommand
        {
            public static readonly LowercaseString IdentifierString = new LowercaseString("ZEROARGUMENTDEBUG");
            public override LowercaseString Identifier => IdentifierString;
        }

        private class OneArgumentDebugLogScriptingCommand : ScriptingCommand
        {
            public static readonly LowercaseString IdentifierString = new LowercaseString("ONEARGUMENTDEBUG");
            public override LowercaseString Identifier => IdentifierString;
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
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(zeroArgumentTokenText, out ParsedTokenSet parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenSet(parsedSet, out LinkedTokenSet linkedTokenSet), "Should be able to link tokens.");

            GameState gameState = new GameState();

            ContextualizedTokenSet contextualizedTokens = new ContextualizedTokenSet(linkedTokenSet);
            Assert.True(GameStateDeltaMaker.TryCreateDelta(contextualizedTokens, gameState, out GameStateDelta generatedDelta), "Should be able to create a game state delta from provided context.");

            Assert.AreEqual(1, generatedDelta.DebugLogs.Count, "Expecting one debug log from the zero argument test.");
            Assert.AreEqual("HELLO!", generatedDelta.DebugLogs[0], "Expecting debug log to be as designated.");
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
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenSet parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenSet(parsedSet, out LinkedTokenSet linkedTokenSet), "Should be able to link tokens.");

            GameState gameState = new GameState();

            ContextualizedTokenSet contextualizedTokens = new ContextualizedTokenSet(linkedTokenSet);
            Assert.True(GameStateDeltaMaker.TryCreateDelta(contextualizedTokens, gameState, out GameStateDelta generatedDelta), "Should be able to create a game state delta from provided context.");

            Assert.AreEqual(1, generatedDelta.DebugLogs.Count, "Expecting one debug log from the one argument test.");
            Assert.AreEqual("111", generatedDelta.DebugLogs[0], "Expecting debug log to be as designated.");
        }
    }
}