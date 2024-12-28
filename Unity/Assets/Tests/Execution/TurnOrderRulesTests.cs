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
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Instances;

    /// <summary>
    /// Tests relating to the turn-order ruleset, used in Slay the Spire games.
    /// 
    /// - Encounter Start: Make the lowest index faction start turn.
    /// - Faction Turn Start: Make the lowest index faction member start turn.
    /// - Entity Turn Start: ~~no op~~
    /// - Entity Turn End: Pick the next faction turn member to start turn. Otherwise, end fation turn.
    /// - Faction Turn End: Make next faction take turn.
    /// </summary>
    public class TurnOrderRulesTests
    {
        [TearDown]
        public void TearDown()
        {
            CommonTestUtility.TearDownDatabases();
        }


        /// <summary>
        /// When the encounter starts, the first faction should start their turn.
        /// This should lead to it becoming the turn of the first faction member.
        /// </summary>
        [Test]
        public void EncounterStarts_FirstFactionTurn()
        {
            // ARRANGE
            RuleReference.RegisterRule(new EncounterStartPlayerTurnRule());
            RuleReference.RegisterRule(new FactionStartsFirstTurnRule());
            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();

            Entity factionOneEntityFirst = new Entity();
            factionOneEntityFirst.SetQuality(WellknownQualities.Faction, WellknownFactions.Player);
            encounter.EncounterEntities.Add(factionOneEntityFirst);
            Entity factionOneEntitySecond = new Entity();
            factionOneEntitySecond.SetQuality(WellknownQualities.Faction, WellknownFactions.Player);
            encounter.EncounterEntities.Add(factionOneEntitySecond);

            Entity factionTwoEntity = new Entity();
            factionTwoEntity.SetQuality(WellknownQualities.Faction, WellknownFactions.Foe);
            encounter.EncounterEntities.Add(factionTwoEntity);

            // ACT
            gameState.StartEncounter(encounter);
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.True(gameState.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(gameState, out Entity currentTurnEntity), "Should have an entity who is currently taking their turn.");
            Assert.AreEqual(factionOneEntityFirst, currentTurnEntity, "It should be the second entity on the player faction's turn.");
        }

        /// <summary>
        /// Sets up an encounter, then ends the turn of the current entity.
        /// The teammates that is next should start their turn.
        /// </summary>
        [Test]
        public void EntityTurnEnd_NextFactionMemberStarts()
        {
            // ARRANGE
            RuleReference.RegisterRule(new EncounterStartPlayerTurnRule());
            RuleReference.RegisterRule(new FactionStartsFirstTurnRule());
            RuleReference.RegisterRule(new TurnEndNextAllyOrEndFactionTurnRule());
            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();

            Entity factionOneEntityFirst = new Entity();
            factionOneEntityFirst.SetQuality(WellknownQualities.Faction, WellknownFactions.Player);
            encounter.EncounterEntities.Add(factionOneEntityFirst);
            Entity factionOneEntitySecond = new Entity();
            factionOneEntitySecond.SetQuality(WellknownQualities.Faction, WellknownFactions.Player);
            encounter.EncounterEntities.Add(factionOneEntitySecond);

            Entity factionTwoEntity = new Entity();
            factionTwoEntity.SetQuality(WellknownQualities.Faction, WellknownFactions.Foe);
            encounter.EncounterEntities.Add(factionTwoEntity);

            // ACT
            gameState.StartEncounter(encounter);
            PendingResolveExecutor.ResolveAll(gameState);
            gameState.EndCurrentEntityTurn();
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.True(gameState.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(gameState, out Entity currentTurnEntity), "Should have an entity who is currently taking their turn.");
            Assert.AreEqual(factionOneEntitySecond, currentTurnEntity, "It should be the second entity on the player faction's turn.");
        }

        /// <summary>
        /// Sets up an encounter, then ends the turn of the first faction member.
        /// It should become the next faction's turn.
        /// </summary>
        [Test]
        public void FactionTurnEnd_NextFaction()
        {
            // ARRANGE
            RuleReference.RegisterRule(new EncounterStartPlayerTurnRule());
            RuleReference.RegisterRule(new FactionStartsFirstTurnRule());
            RuleReference.RegisterRule(new TurnEndNextAllyOrEndFactionTurnRule());
            RuleReference.RegisterRule(new FactionEndTurnNextFactionRule());
            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();

            Entity factionOneEntity = new Entity();
            factionOneEntity.SetQuality(WellknownQualities.Faction, WellknownFactions.Player);
            encounter.EncounterEntities.Add(factionOneEntity);

            Entity factionTwoEntity = new Entity();
            factionTwoEntity.SetQuality(WellknownQualities.Faction, WellknownFactions.Foe);
            encounter.EncounterEntities.Add(factionTwoEntity);

            // ACT
            gameState.StartEncounter(encounter);
            PendingResolveExecutor.ResolveAll(gameState);
            gameState.EndCurrentEntityTurn();
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.AreEqual(WellknownFactions.Foe, gameState.FactionTurnTakerCalculator.GetCurrentFaction(), "It should be the enemy faction's turn.");
            Assert.True(gameState.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(gameState, out Entity currentTurnEntity), "Should have an entity who is currently taking their turn.");
            Assert.AreEqual(factionTwoEntity, currentTurnEntity, "It should be the foe faction's entity's turn.");
        }

        /// <summary>
        /// Sets up an encounter, the ends the turn of the two entities in play.
        /// It should become the player's turn again, going all around the way.
        /// </summary>
        [Test]
        public void EndTurn_AllAroundBackToPlayer()
        {
            // ARRANGE
            RuleReference.RegisterRule(new EncounterStartPlayerTurnRule());
            RuleReference.RegisterRule(new FactionStartsFirstTurnRule());
            RuleReference.RegisterRule(new TurnEndNextAllyOrEndFactionTurnRule());
            RuleReference.RegisterRule(new FactionEndTurnNextFactionRule());
            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();

            Entity factionOneEntity = new Entity();
            factionOneEntity.SetQuality(WellknownQualities.Faction, WellknownFactions.Player);
            encounter.EncounterEntities.Add(factionOneEntity);

            Entity factionTwoEntity = new Entity();
            factionTwoEntity.SetQuality(WellknownQualities.Faction, WellknownFactions.Foe);
            encounter.EncounterEntities.Add(factionTwoEntity);

            // ACT
            gameState.StartEncounter(encounter);
            PendingResolveExecutor.ResolveAll(gameState);
            gameState.EndCurrentEntityTurn();
            PendingResolveExecutor.ResolveAll(gameState);
            gameState.EndCurrentEntityTurn();
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.AreEqual(WellknownFactions.Player, gameState.FactionTurnTakerCalculator.GetCurrentFaction(), "It should be the player faction's turn.");
            Assert.True(gameState.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(gameState, out Entity currentTurnEntity), "Should have an entity who is currently taking their turn.");
            Assert.AreEqual(factionOneEntity, currentTurnEntity, "It should be the player faction's entity's turn.");
        }

        /// <summary>
        /// Sets up an encounter. The player should draw a starting hand of cards.
        /// </summary>
        [Test]
        public void StartTurn_DrawHandOfCards()
        {
            // ARRANGE
            RuleReference.RegisterRule(new EncounterStartCopyDeckRule());
            RuleReference.RegisterRule(new EncounterStartPlayerTurnRule());
            RuleReference.RegisterRule(new FactionStartsFirstTurnRule());
            decimal cardsToDraw = 7;
            RuleReference.RegisterRule(new PlayerTurnStartDrawCardsRule(new ConstantNumericValue(cardsToDraw)));
            GameState gameState = new GameState();

            for (int ii = 0; ii < cardsToDraw; ii ++)
            {
                gameState.AddCard(new CardInstance(), WellknownZones.Campaign);
            }

            EncounterState encounter = new EncounterState();

            Entity playerEntity = new Entity();
            playerEntity.SetQuality(WellknownQualities.Faction, WellknownFactions.Player);
            encounter.EncounterEntities.Add(playerEntity);

            // ACT
            gameState.StartEncounter(encounter);
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.AreEqual(cardsToDraw, gameState.GetCardsInZone(WellknownZones.Hand).Count, "Expecting exactly seven cards in hand.");
            Assert.AreEqual(0, gameState.GetCardsInZone(WellknownZones.Deck).Count, "Because the deck contained exactly seven cards, it should now be empty.");
            Assert.AreEqual(0, gameState.GetCardsInZone(WellknownZones.Exile).Count, "The exile zone should not have any cards in it.");
            Assert.AreEqual(0, gameState.GetCardsInZone(WellknownZones.Discard).Count, "The discard zone should not have any cards in it.");
        }

        /// <summary>
        /// This test builds on <see cref="StartTurn_DrawHandOfCards"/>, continuing it by
        /// discarding the cards in hand at the end of turn.
        /// </summary>
        [Test]
        public void EndTurn_DiscardHandOfCards()
        {
            // ARRANGE
            RuleReference.RegisterRule(new EncounterStartCopyDeckRule());
            RuleReference.RegisterRule(new EncounterStartPlayerTurnRule());
            RuleReference.RegisterRule(new FactionStartsFirstTurnRule());
            decimal cardsToDraw = 7;
            RuleReference.RegisterRule(new PlayerTurnStartDrawCardsRule(new ConstantNumericValue(cardsToDraw)));
            RuleReference.RegisterRule(new PlayerTurnEndDiscardRule());
            GameState gameState = new GameState();

            for (int ii = 0; ii < cardsToDraw; ii++)
            {
                gameState.AddCard(new CardInstance(), WellknownZones.Campaign);
            }

            EncounterState encounter = new EncounterState();

            Entity playerEntity = new Entity();
            playerEntity.SetQuality(WellknownQualities.Faction, WellknownFactions.Player);
            encounter.EncounterEntities.Add(playerEntity);

            // ACT
            gameState.StartEncounter(encounter);
            PendingResolveExecutor.ResolveAll(gameState);
            gameState.EndCurrentEntityTurn();
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.AreEqual(0, gameState.GetCardsInZone(WellknownZones.Hand).Count, "The player should have discarded their hand, so it should be empty.");
            Assert.AreEqual(0, gameState.GetCardsInZone(WellknownZones.Deck).Count, "Because the deck contained exactly seven cards, it should now be empty.");
            Assert.AreEqual(0, gameState.GetCardsInZone(WellknownZones.Exile).Count, "The exile zone should not have any cards in it.");
            Assert.AreEqual(cardsToDraw, gameState.GetCardsInZone(WellknownZones.Discard).Count, "The discarded hand should be entirely in the discard.");
        }
    }
}