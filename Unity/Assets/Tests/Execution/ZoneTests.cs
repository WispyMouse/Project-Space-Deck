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

        /// <summary>
        /// A card that is played should have its destination set, and then moved to that destination.
        /// This is by default with these rules the discard pile.
        /// </summary>
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
            LinkedCardInstance cardInstance = new LinkedCardInstance(cardPrototype, ElementDatabase.Provider);
            gameState.StartEncounter(encounter);
            gameState.AddCard(cardInstance, WellknownZones.Hand);

            // ACT
            gameState.StartConsideringPlayingCard(cardInstance);
            Assert.IsTrue(gameState.TryExecuteCurrentCard(new ExecutionAnswerSet(user: null)), "Should be able to execute question-less card.");
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.IsTrue(gameState.GetCardZone(cardInstance) == WellknownZones.Discard);
        }

        /// <summary>
        /// This creates a card that sets its own destination after playing.
        /// It should move to that destination, not the default discard pile.
        /// This destination is the exile zone.
        /// </summary>
        [Test]
        public void PlayedCard_SetExileDestination_Moved()
        {
            // ARRANGE
            RuleReference.RegisterRule(new PlayedCardsAreDiscardedRule());
            RuleReference.RegisterRule(new MovePlayedCardToDestinationRule());
            ScriptingCommandReference.RegisterScriptingCommand(new SetDestinationScriptingCommand());
            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            CardImport import = new CardImport()
            {
                Id = nameof(PlayedCard_SetExileDestination_Moved),
                Name = nameof(PlayedCard_SetExileDestination_Moved),
                EffectScript = "[DESTINATION:EXILE]"
            };
            CardPrototype cardPrototype = import.GetPrototype();
            CardDatabase.RegisterCardPrototype(cardPrototype);
            CardDatabase.LinkTokens();
            LinkedCardInstance cardInstance = new LinkedCardInstance(cardPrototype, ElementDatabase.Provider);
            gameState.StartEncounter(encounter);
            gameState.AddCard(cardInstance, WellknownZones.Hand);

            // ACT
            gameState.StartConsideringPlayingCard(cardInstance);
            Assert.IsTrue(gameState.TryExecuteCurrentCard(new ExecutionAnswerSet(user: null)), "Should be able to execute question-less card.");
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.IsTrue(gameState.GetCardZone(cardInstance) == WellknownZones.Exile);
        }

        /// <summary>
        /// This creates a card that sets its own destination after playing.
        /// It should move to that destination, not the default discard pile.
        /// This destination is back into the hand. That's generally against
        /// the normal flow of things, so this test validates that continues
        /// to work.
        /// </summary>
        [Test]
        public void PlayedCard_SetHandDestination_Moved()
        {
            // ARRANGE
            RuleReference.RegisterRule(new PlayedCardsAreDiscardedRule());
            RuleReference.RegisterRule(new MovePlayedCardToDestinationRule());
            ScriptingCommandReference.RegisterScriptingCommand(new SetDestinationScriptingCommand());
            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            CardImport import = new CardImport()
            {
                Id = nameof(PlayedCard_SetHandDestination_Moved),
                Name = nameof(PlayedCard_SetHandDestination_Moved),
                EffectScript = "[DESTINATION:HAND]"
            };
            CardPrototype cardPrototype = import.GetPrototype();
            CardDatabase.RegisterCardPrototype(cardPrototype);
            CardDatabase.LinkTokens();
            LinkedCardInstance cardInstance = new LinkedCardInstance(cardPrototype, ElementDatabase.Provider);
            gameState.StartEncounter(encounter);
            gameState.AddCard(cardInstance, WellknownZones.Hand);

            // ACT
            gameState.StartConsideringPlayingCard(cardInstance);
            Assert.IsTrue(gameState.TryExecuteCurrentCard(new ExecutionAnswerSet(user: null)), "Should be able to execute question-less card.");
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.IsTrue(gameState.GetCardZone(cardInstance) == WellknownZones.Exile);
        }

        /// <summary>
        /// This creates an encounter with a deck of cards, and exiles one of them.
        /// Then the deck is instructed to shuffle.
        /// The exile zone should not shuffle into the deck.
        /// </summary>
        [Test]
        public void ExiledCards_DoNotShuffleIn()
        {
            int cardsInDeck = 10;

            // ARRANGE
            ScriptingCommandReference.RegisterScriptingCommand(new SetDestinationScriptingCommand());
            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            CardImport import = new CardImport()
            {
                Id = nameof(ExiledCards_DoNotShuffleIn),
                Name = nameof(ExiledCards_DoNotShuffleIn),
                EffectScript = ""
            };
            CardPrototype cardPrototype = import.GetPrototype();
            CardDatabase.RegisterCardPrototype(cardPrototype);
            CardDatabase.LinkTokens();
            LinkedCardInstance cardInstance = new LinkedCardInstance(cardPrototype, ElementDatabase.Provider);
            gameState.StartEncounter(encounter);

            // ACT
            for (int ii = 0; ii < cardsInDeck; ii++)
            {
                gameState.AddCard(new LinkedCardInstance(cardPrototype, ElementDatabase.Provider), WellknownZones.Deck);
            }
            CardInstance exileThis = gameState.GetCardsInZone(WellknownZones.Deck)[0];
            gameState.MoveCard(exileThis, WellknownZones.Exile);
            gameState.ShuffleDiscardAndDeck();

            // ASSERT
            Assert.AreEqual(cardsInDeck - 1, gameState.GetCardsInZone(WellknownZones.Deck).Count, "There should be exactly one card missing from the deck.");
            Assert.AreEqual(WellknownZones.Exile, gameState.GetCardZone(exileThis), "The exiled card should still be in exile.");
        }

        /// <summary>
        /// This validates that you can add a card to a campaign deck.
        /// </summary>
        [Test]
        public void CampaignDeck_AddCard()
        {
            // ARRANGE
            CardImport import = new CardImport()
            {
                Id = nameof(CampaignDeck_AddCard),
                Name = nameof(CampaignDeck_AddCard),
                EffectScript = ""
            };
            CardPrototype cardPrototype = import.GetPrototype();
            CardDatabase.RegisterCardPrototype(cardPrototype);
            CardDatabase.LinkTokens();
            GameState gameState = new GameState();

            // ACT
            CardInstance addedCard = CardDatabase.GetInstance(import.Id);
            gameState.AddCard(addedCard, WellknownZones.Campaign);

            // ASSERT
            Assert.AreEqual(1, gameState.CardsInDeck.Count, "Should be exactly one card in the deck.");
            Assert.AreEqual(addedCard, gameState.CardsInDeck[0], "The added card should be as expected.");
        }

        [Test]
        public void CampaignDeck_ShufflesAsEncounterStarts()
        {
            // ARRANGE
            RuleReference.RegisterRule(new EncounterStartCopyDeckRule());
            CardImport import = new CardImport()
            {
                Id = nameof(CampaignDeck_ShufflesAsEncounterStarts),
                Name = nameof(CampaignDeck_ShufflesAsEncounterStarts),
                EffectScript = ""
            };
            CardPrototype cardPrototype = import.GetPrototype();
            CardDatabase.RegisterCardPrototype(cardPrototype);
            CardDatabase.LinkTokens();
            GameState gameState = new GameState();
            CardInstance addedCard = CardDatabase.GetInstance(import.Id);
            gameState.AddCard(addedCard, WellknownZones.Campaign);

            // ACT
            EncounterState encounter = new EncounterState();
            gameState.StartEncounter(encounter);
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.AreEqual(1, gameState.GetCardsInZone(WellknownZones.Deck).Count, "Should have exactly one card in the encounter's deck.");
            Assert.AreEqual(addedCard, gameState.GetCardsInZone(WellknownZones.Deck)[0], "The card in the deck should be the same instance.");
        }
    }
}