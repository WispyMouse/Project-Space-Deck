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
    using SpaceDeck.Tests.EditMode.TestFixtures;

    public class StatusEffectsTests
    {
        [TearDown]
        public void TearDown()
        {
            CommonTestUtility.TearDownDatabases();
        }

        /// <summary>
        /// Plays an effect on an entity that should apply status effects.
        /// Asserts that the stacks are applied.
        /// </summary>
        public void Stacks_Applied()
        {
            LowercaseString debugStatusId = "DEBUG";
            int stacksToApply = 4;

            // ARRANGE
            var applyStatusEffectStacksScriptingCommand = new ApplyStatusEffectStacksScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(applyStatusEffectStacksScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());

            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            Entity targetingEntity = new Entity();
            encounter.EncounterEntities.Add(targetingEntity);
            gameState.StartEncounter(encounter);

            // ACT
            string damageArgumentTokenTextString = $"[{applyStatusEffectStacksScriptingCommand.Identifier}:{debugStatusId} {stacksToApply}]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(damageArgumentTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");
            ExecutionAnswerSet answers = new ExecutionAnswerSet(new EffectTargetExecutionAnswer(linkedTokenSet.GetQuestions()[0], targetingEntity));
            Assert.True(GameStateDeltaMaker.TryCreateDelta(linkedTokenSet, answers, gameState, out GameStateDelta generatedDelta), "Should be able to create delta after providing answers.");

            // ASSERT
            // With the appropriate answers provided, assert this can be performed
            Assert.IsTrue(targetingEntity.AppliedStatusEffects.ContainsKey(debugStatusId), "Should contain the debug status.");
            Assert.AreEqual(stacksToApply, targetingEntity.AppliedStatusEffects[debugStatusId].Qualities.GetNumericQuality(WellknownQualities.Stacks), "Should have the expected number of applied stacks.");
        }
    }
}