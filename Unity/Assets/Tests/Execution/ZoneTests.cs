namespace SpaceDeck.Tests.EditMode.Execution
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
    using System.Linq;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Tests.EditMode.Common.TestFixtures;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Databases;

    public class ZoneTests
    {
        [TearDown]
        public void TearDown()
        {
            CommonTestUtility.TearDownDatabases();
        }

        [Test]
        public void PlayedCard_IsDiscarded()
        {
            // ARRANGE
            RuleReference.RegisterRule(new PlayedCardsAreDiscardedRule());
            RuleReference.RegisterRule(new MovePlayedCardToDestinationRule());
            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            CardImport import = new CardImport()
            {
                Id = nameof(PlayedCard_IsDiscarded),
                Name = nameof(PlayedCard_IsDiscarded),
                EffectScript = String.Empty
            };
            CardPrototype cardPrototype = import.GetPrototype();
            CardDatabase.RegisterCardPrototype(cardPrototype);
            CardDatabase.LinkTokens();
            LinkedCardInstance cardInstance = new LinkedCardInstance(cardPrototype);
            gameState.StartEncounter(encounter);
            gameState.AddCard(cardInstance, WellknownZones.Hand);

            // ACT
            gameState.StartConsideringPlayingCard(cardInstance);
            Assert.IsTrue(gameState.TryExecuteCurrentCard(), "Should be able to execute question-less card.");
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.IsTrue(gameState.GetCardZone(cardInstance) == WellknownZones.Discard);
        }
    }
}