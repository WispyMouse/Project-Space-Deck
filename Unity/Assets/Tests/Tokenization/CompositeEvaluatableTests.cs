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
    using System.Linq;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.Tests.EditMode.Common.TestFixtures;

    public class CompositeEvaluatableTests
    {
        [TearDown]
        public void TearDown()
        {
            CommonTestUtility.TearDownDatabases();
            LoggingGameStateChange.LastLog = String.Empty;
        }

        [Test]
        public void Division_RoundsDown()
        {
            // ARRANGE
            ScriptingCommandReference.RegisterScriptingCommand(new OneArgumentDebugLogScriptingCommand());
            CardImport import = new CardImport()
            {
                Id = nameof(Division_RoundsDown),
                EffectScript = $"[{OneArgumentDebugLogScriptingCommand.IdentifierString}:10/3]"
            };
            CardDatabase.AddCardToDatabase(import);
            CardInstance instance = CardDatabase.GetInstance(import.Id);

            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            
            gameState.StartEncounter(encounter);

            // ACT
            gameState.StartConsideringPlayingCard(instance);
            Assert.IsTrue(gameState.TryExecuteCurrentCard(), "Should be able to execute question-less card.");
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.AreEqual(3, LoggingGameStateChange.LastLog, "Expecting the last log to be 3, which is 10/3 rounded down.");
        }

        /// <summary>
        /// This does a random amount of damage.
        /// After an 'appropriate' amount of time, we should be able to expect
        /// every number to be hit if things are working right.
        /// </summary>
        [Test]
        public void Randomization_RoundTrip()
        {
            int timesToPlayCard = 1000;

            // ARRANGE
            ScriptingCommandReference.RegisterScriptingCommand(new OneArgumentDebugLogScriptingCommand());
            CardImport import = new CardImport()
            {
                Id = nameof(Division_RoundsDown),
                EffectScript = $"[{OneArgumentDebugLogScriptingCommand.IdentifierString}:0~10]"
            };
            CardDatabase.AddCardToDatabase(import);
            CardInstance instance = CardDatabase.GetInstance(import.Id);

            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();

            gameState.StartEncounter(encounter);
            HashSet<int> allNumbers = new HashSet<int>();

            // ACT
            for (int ii = 0; ii < timesToPlayCard; ii++)
            {
                gameState.StartConsideringPlayingCard(instance);
                Assert.IsTrue(gameState.TryExecuteCurrentCard(), "Should be able to execute question-less card.");
                PendingResolveExecutor.ResolveAll(gameState);

                string lastLog = LoggingGameStateChange.LastLog;
                Assert.IsTrue(int.TryParse(lastLog, out int result), "The last log should be a number that can be cast from int.");
                allNumbers.Add(ii);

                if (allNumbers.Count == 11)
                {
                    break;
                }
            }

            // ASSERT
            for (int ii = 0; ii <= 10; ii++)
            {
                Assert.IsTrue(allNumbers.Contains(ii), "All numbers from 0-10 (inclusive) should be eventually hit.");
            }
        }
    }
}