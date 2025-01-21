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
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Tests.EditMode.Common.TestFixtures;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.GameState.Deltas;

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
        [Test]
        public void Stacks_Applied()
        {
            LowercaseString debugStatusId = "DEBUG";
            int stacksToApply = 4;

            // ARRANGE
            var applyStatusEffectStacksScriptingCommand = new ApplyStatusEffectStacksScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(applyStatusEffectStacksScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());
            StatusEffectDatabase.RegisterStatusEffectPrototype(new StatusEffectPrototype(debugStatusId, debugStatusId.ToString()));

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
            ExecutionAnswerSet answers = new ExecutionAnswerSet(new EffectTargetExecutionAnswer(linkedTokenSet.GetQuestions()[0], targetingEntity), targetingEntity);
            Assert.True(GameStateDeltaMaker.TryCreateDelta(linkedTokenSet, answers, gameState, out GameStateDelta generatedDelta), "Should be able to create delta after providing answers.");
            GameStateDeltaApplier.ApplyGameStateDelta(gameState, generatedDelta);
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            // With the appropriate answers provided, assert this can be performed
            Assert.IsTrue(targetingEntity.AppliedStatusEffects.ContainsKey(debugStatusId), "Should contain the debug status.");
            Assert.AreEqual(stacksToApply, targetingEntity.AppliedStatusEffects[debugStatusId].Qualities.GetNumericQuality(WellknownQualities.Stacks), "Should have the expected number of applied stacks.");
        }

        /// <summary>
        /// Registers a status effect that fires when an entity starts their turn.
        /// </summary>
        [Test]
        public void AppliedStatus_Procs_StartTurn()
        {
            LowercaseString debugStatusId = "DEBUG";
            int stacksToApply = 4;
            bool debugValue = false;

            // ARRANGE
            var applyStatusEffectStacksScriptingCommand = new ApplyStatusEffectStacksScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(applyStatusEffectStacksScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());
            RuleReference.RegisterRule(new EncounterStartPlayerTurnRule());
            RuleReference.RegisterRule(new FactionStartsFirstTurnRule());
            ExecuteAppliedStatusEffectPrototype prototype = new ExecuteAppliedStatusEffectPrototype(debugStatusId, debugStatusId.ToString());
            StatusEffectDatabase.RegisterStatusEffectPrototype(prototype);
            StatusEffectDatabase.RegisterStatusEffectPrototypeFactory(prototype,
                (StatusEffectPrototype Prototype) =>
                {
                    return new ExecuteAppliedStatusEffectInstance(
                        (IGameStateMutator Mutator) => { debugValue = true; },
                        new List<LowercaseString>() { WellknownGameStateEvents.EntityTurnStarted },
                        debugStatusId.ToString(),
                        debugStatusId);
                });

            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            Entity targetingEntity = new Entity();
            encounter.EncounterEntities.Add(targetingEntity);

            // ACT
            gameState.StartEncounter(encounter);
            string damageArgumentTokenTextString = $"[{applyStatusEffectStacksScriptingCommand.Identifier}:{debugStatusId} {stacksToApply}]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(damageArgumentTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");
            ExecutionAnswerSet answers = new ExecutionAnswerSet(new EffectTargetExecutionAnswer(linkedTokenSet.GetQuestions()[0], targetingEntity), targetingEntity);
            Assert.True(GameStateDeltaMaker.TryCreateDelta(linkedTokenSet, answers, gameState, out GameStateDelta generatedDelta), "Should be able to create delta after providing answers.");
            GameStateDeltaApplier.ApplyGameStateDelta(gameState, generatedDelta);
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.IsTrue(debugValue, "The beginning of the turn should have triggered the event to set the debug flag to true.");
        }

        /// <summary>
        /// Applies a debug status that resembles poison, and ensure it ticks once.
        /// </summary>
        [Test]
        public void TestDebugPoisonStatus_OneTick()
        {
            int startingHealth = 100;

            // ARRANGE
            RuleReference.RegisterRule(new EncounterStartPlayerTurnRule());
            RuleReference.RegisterRule(new FactionStartsFirstTurnRule());
            StatusEffectImport import = new StatusEffectImport()
            {
                Id = nameof(TestDebugPoisonStatus_OneTick)
            };

            import.Reactors.Add(new ReactorImport()
            {
                TriggerOnEventIds = new List<string>() { WellknownGameStateEvents.GetQualityAffected(WellknownQualities.Health) },
                TokenText = "" // TODO: BLOCK TOKEN TEXT
            });
            StatusEffectDatabase.RegisterStatusEffect(import);
            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            Entity targetingEntity = new Entity();
            gameState.ModStatusEffectStacks(targetingEntity, import.Id, 1);
            targetingEntity.Qualities.SetNumericQuality(WellknownQualities.Health, startingHealth);
            encounter.EncounterEntities.Add(targetingEntity);

            // ACT
            gameState.StartEncounter(encounter);
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.AreEqual(startingHealth - 1, gameState.GetNumericQuality(targetingEntity, WellknownQualities.Health), "Should have lost one health from one stack of poison.");
            Assert.AreEqual(0, gameState.GetStacks(targetingEntity, import.Id), "Should have no stacks after the poison ticks once.");
        }

        /// <summary>
        /// Applies a debug status that resembles poison, with a few stacks.
        /// The turn is passed, and the turn is passed again.
        /// Assert that the poison behaves as expected.
        /// </summary>
        [Test]
        public void TestDebugPoisonStatus_TwoTicks()
        {
            int startingHealth = 100;
            int startingPoison = 10;

            int healthAfterOneTick = startingHealth - startingPoison;
            int healthAfterTwoTicks = startingHealth - startingPoison - (startingPoison - 1);

            // ARRANGE
            RuleReference.RegisterRule(new EncounterStartPlayerTurnRule());
            RuleReference.RegisterRule(new FactionStartsFirstTurnRule());
            RuleReference.RegisterRule(new TurnEndNextAllyOrEndFactionTurnRule());
            RuleReference.RegisterRule(new FactionEndTurnNextFactionRule());

            StatusEffectImport import = new StatusEffectImport()
            {
                Id = nameof(TestDebugPoisonStatus_OneTick)
            };

            import.Reactors.Add(new ReactorImport()
            {
                TriggerOnEventIds = new List<string>() { WellknownGameStateEvents.GetQualityAffected(WellknownQualities.Health) },
                TokenText = "" // TODO: BLOCK TOKEN TEXT
            });
            StatusEffectDatabase.RegisterStatusEffect(import);
            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            Entity targetingEntity = new Entity();
            gameState.ModStatusEffectStacks(targetingEntity, import.Id, startingPoison);
            targetingEntity.Qualities.SetNumericQuality(WellknownQualities.Health, startingHealth);
            encounter.EncounterEntities.Add(targetingEntity);

            // ACT AND ASSERT
            gameState.StartEncounter(encounter);
            PendingResolveExecutor.ResolveAll(gameState);
            Assert.AreEqual(healthAfterOneTick, gameState.GetNumericQuality(targetingEntity, WellknownQualities.Health), "Should have taken a specific amount of damage after one turn.");
            Assert.AreEqual(startingPoison - 1, gameState.GetStacks(targetingEntity, import.Id), "Should have specific stacks after the poison ticks once.");

            gameState.EndCurrentEntityTurn();
            PendingResolveExecutor.ResolveAll(gameState);
            Assert.AreEqual(healthAfterTwoTicks, gameState.GetNumericQuality(targetingEntity, WellknownQualities.Health), "Should have taken a specific amount of damage after two turns.");
            Assert.AreEqual(startingPoison - 2, gameState.GetStacks(targetingEntity, import.Id), "Should have specific stacks after the poison ticks twice.");
        }
    }
}