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

    /// <summary>
    /// These tests cover the experience of a player attempting to play a card.
    /// This is to ensure that "playing cards" functions appropriately.
    /// </summary>
    public class PlayingCardsTests
    {
        private class ExecuteScriptingCommand : ScriptingCommand
        {
            public static readonly LowercaseString IdentifierString = new LowercaseString("ExecuteScriptingCommand");
            public override LowercaseString Identifier => IdentifierString;

            public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
            {
                throw new System.NotImplementedException("You should not parse this token. Instead, directly create an ExecuteLinkedToken.");
            }
        }

        private class ExecuteLinkedToken : LinkedToken<ExecuteScriptingCommand>
        {
            public readonly Action<IGameStateMutator> Action;

            public ExecuteLinkedToken(Action<IGameStateMutator> action) : base()
            {
                this.Action = action;
            }

            public override bool TryGetChanges(ScriptingExecutionContext context, out List<GameStateChange> changes)
            {
                changes = new List<GameStateChange>();
                changes.Add(new ActionExecutor(this.Action));
                return true;
            }
        }

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
            gameState.StartPlayCard(cardInstance);
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.IsTrue(debugValue, "The card should have successfully played, resulting in the action changing the debug value to true.");
            Assert.IsTrue(encounter.CardsInZones[cardInstance] == WellknownZones.Discard, "The played card should now be in the discard pile.");
        }
    }
}