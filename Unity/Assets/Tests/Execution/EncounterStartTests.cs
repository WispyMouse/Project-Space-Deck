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

    public class EncounterStartTests
    {
        [TearDown]
        public void TearDown()
        {
            CommonTestUtility.TearDownDatabases();
        }

        /// <summary>
        /// This is a simple test that executes adding a character to an encounter,
        /// and ensures they were added by the effect.
        /// </summary>
        [Test]
        public void Add_PlayerPersistentCharacter()
        {
            // ARRANGE
            Entity playerEntity = new Entity();
            GameState gameState = new GameState();

            // ACT
            gameState.AddPersistentEntity(playerEntity);

            // ASSERT
            Assert.IsTrue(gameState.PersistentEntities.Contains(playerEntity), "Persistent entities should contain the player.");
            Assert.IsTrue(gameState.GetAllEntities().Contains(playerEntity), "All entities should contain the player.");
            Assert.IsTrue(gameState.EntityIsAlive(playerEntity), "The player should be considered alive, as they were added to the game state.");
        }

        /// <summary>
        /// This test adds a character to a game state, and then creates a delta off of an empty effect.
        /// The delta is told to remove the character.
        /// The character should be considered removed by the delta, but the core game state should be unaffected.
        /// </summary>
        [Test]
        public void GameStateDelta_RemoveCharacter_StillPresentInCoreGameState()
        {
            // ARRANGE
            Entity playerEntity = new Entity();
            GameState gameState = new GameState();
            gameState.AddPersistentEntity(playerEntity);
            GameStateDelta delta = new GameStateDelta(gameState);

            // ACT
            delta.RemoveEntity(playerEntity);

            // ASSERT
            Assert.IsTrue(gameState.EntityIsAlive(playerEntity), $"In {nameof(gameState)} the player should be considered alive, as they were added to the game state.");
            Assert.IsFalse(delta.EntityIsAlive(playerEntity), $"In {nameof(delta)} the player should not be considered alive, as they were removed.");
            Assert.IsFalse(delta.GetAllEntities().Contains(playerEntity), "The removed entity should not be present in the all entities list.");
            Assert.IsTrue(gameState.PersistentEntities.Contains(playerEntity), "The player should not be missing from the core game state.");
        }
    }
}
