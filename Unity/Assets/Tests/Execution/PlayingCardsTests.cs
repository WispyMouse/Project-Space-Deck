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
    using SpaceDeck.Models.Databases;

    /// <summary>
    /// These tests cover the experience of a player attempting to play a card.
    /// This is to ensure that "playing cards" functions appropriately.
    /// </summary>
    public class PlayingCardsTests
    {
        [TearDown]
        public void TearDown()
        {
            CommonTestUtility.TearDownDatabases();
        }

        /// <summary>
        /// Creates a card that changes a variable.
        /// This validates that playing the card does execute things.
        /// This particular card has no 'questions', and no targets,
        /// so it'll execute immediately.
        /// </summary>
        [Test]
        public void PlayCard_NoQuestions_DebugExecutes()
        {
            // ARRANGE
            RuleReference.RegisterRule(new PlayedCardsAreDiscardedRule());
            RuleReference.RegisterRule(new MovePlayedCardToDestinationRule());
            bool debugValue = false;
            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            CardPrototype cardPrototype = new CardPrototype(
                nameof(PlayCard_NoQuestions_DebugExecutes),
                LinkedTokenMaker.CreateTokenListFromLinkedTokens(
                    new ExecuteLinkedToken((IGameStateMutator mutator) => { debugValue = true; }))
                );
            LinkedCardInstance cardInstance = new LinkedCardInstance(cardPrototype);
            gameState.StartEncounter(encounter);
            gameState.AddCard(cardInstance, WellknownZones.Hand);

            // ACT
            gameState.StartConsideringPlayingCard(cardInstance);
            Assert.IsTrue(gameState.TryExecuteCurrentCard(), "Should be able to execute question-less card.");
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.IsTrue(debugValue, "The card should have successfully played, resulting in the action changing the debug value to true.");
            Assert.IsTrue(encounter.CardsInZones[cardInstance] == WellknownZones.Discard, "The played card should now be in the discard pile.");
        }

        /// <summary>
        /// Creates a card that changes a variable, which requires a single target.
        /// This validates that playing the card does execute things.
        /// This particular card has a question, but it can be played automatically.
        /// </summary>
        [Test]
        public void PlayCard_OneQuestion_AutoExecutes()
        {
            // ARRANGE
            RuleReference.RegisterRule(new PlayedCardsAreDiscardedRule());
            RuleReference.RegisterRule(new MovePlayedCardToDestinationRule());
            bool debugValue = false;
            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            CardPrototype cardPrototype = new CardPrototype(
                nameof(PlayCard_OneQuestion_AutoExecutes),
                LinkedTokenMaker.CreateTokenListFromLinkedTokens(
                    new ExecuteWithTargetLinkedToken(new ChangeTargetEvaluatableValue(FoeTargetProvider.Instance), (IGameStateMutator mutator) => { debugValue = true; }))
                );
            LinkedCardInstance cardInstance = new LinkedCardInstance(cardPrototype);
            Entity foeEntity = new Entity();
            foeEntity.Qualities.SetNumericQuality(WellknownQualities.Faction, WellknownFactions.Foe);
            encounter.EncounterEntities.Add(foeEntity);
            gameState.StartEncounter(encounter);
            gameState.AddCard(cardInstance, WellknownZones.Hand);
            IndexChoosingAnswerer answerer = new IndexChoosingAnswerer(0);

            // ACT
            QuestionAnsweringContext answeringContext = gameState.StartConsideringPlayingCard(cardInstance);
            Assert.IsTrue(gameState.TryGetCurrentQuestions(out IReadOnlyList<ExecutionQuestion> questions), "The game state should have one current question, because this card requires a target.");
            ExecutionAnswerSet answers = null;
            answerer.HandleQuestions(answeringContext, questions, (ExecutionAnswerSet handledAnswer) =>
            {
                answers = handledAnswer;
            });
            Assert.IsTrue(gameState.TryExecuteCurrentCard(answers), "With the question answered, the card should be ready to play.");
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.IsTrue(debugValue, "The card should have successfully played, resulting in the action changing the debug value to true.");
            Assert.IsTrue(encounter.CardsInZones[cardInstance] == WellknownZones.Discard, "The played card should now be in the discard pile.");
        }
    }
}