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
    using System.Linq;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.GameState.Deltas;

    public class BeforeDirectionTests
    {
        [TearDown]
        public void TearDown()
        {
            CommonTestUtility.TearDownDatabases();
        }

        public class Before_ReducesIntensity_DataSource_Object
        {
            public int StartingHealth;
            public int DamageReduction;
            public int Damage;

            public int ExpectedEndHealth;

            public Before_ReducesIntensity_DataSource_Object(int startingHealth, int damage, int damageReduction)
            {
                this.StartingHealth = startingHealth;
                this.Damage = damage;
                this.DamageReduction = damageReduction;

                this.ExpectedEndHealth = startingHealth - Mathf.Max(0, damage - DamageReduction);
            }

            public override string ToString()
            {
                return $"StartingHealth {StartingHealth} // DamageReduction {DamageReduction} // Damage {Damage} // ExpectedEndHealth {ExpectedEndHealth}";
            }
        }

        public static Before_ReducesIntensity_DataSource_Object[] Before_LowersValue_DataSource =
        {
            new Before_ReducesIntensity_DataSource_Object(100, 10, 5),
            new Before_ReducesIntensity_DataSource_Object(100, 10, 15),
            new Before_ReducesIntensity_DataSource_Object(100, 10, 0),
            new Before_ReducesIntensity_DataSource_Object(100, 10, 15),
            new Before_ReducesIntensity_DataSource_Object(10, 10, 15)
        };

        /// <summary>
        /// This sets up a situation where health would be lowered, but an
        /// attached status effect is going to reduce the intensity first.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(Before_LowersValue_DataSource))]
        public void Before_ReducesIntensity(Before_ReducesIntensity_DataSource_Object data)
        {
            // ARRANGE
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());
            DamageScriptingCommand damageScriptingCommand = new DamageScriptingCommand();
            ReduceIntensityScriptingCommand reduceScriptingCommand = new ReduceIntensityScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(damageScriptingCommand);
            ScriptingCommandReference.RegisterScriptingCommand(reduceScriptingCommand);

            StatusEffectImport import = new StatusEffectImport()
            {
                Id = nameof(Before_ReducesIntensity),
                Reactors = new List<ReactorImport>()
                {
                    new ReactorImport()
                    {
                         Direction = GameStateEventTrigger.TriggerDirection.Before,
                         TokenText = $"[{reduceScriptingCommand.Identifier}:1]",
                         TriggerOnEventIds = new List<string>()
                         {
                             WellknownGameStateEvents.GetQualityAffected(WellknownQualities.Health)
                         }
                    }
                }
            };
            StatusEffectDatabase.RegisterStatusEffect(import);
            AllDatabases.LinkAllDatabase();

            GameState gameState = new GameState();
            EncounterState encounterState = new EncounterState();
            Entity targetingEntity = new Entity();
            targetingEntity.Qualities.SetNumericQuality(WellknownQualities.Health, data.StartingHealth);
            gameState.ModStatusEffectStacks(targetingEntity, import.Id, data.DamageReduction);
            encounterState.EncounterEntities.Add(targetingEntity);
            gameState.StartEncounter(encounterState);
            PendingResolveExecutor.ResolveAll(gameState);

            string damageArgumentTokenTextString = $"[{damageScriptingCommand.Identifier}:{data.Damage}]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(damageArgumentTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");
            Assert.True(linkedTokenSet.Scopes.Count == 1 && linkedTokenSet.Scopes[0].Tokens.Count == 1 && linkedTokenSet.Scopes[0].Tokens[0] is DamageLinkedToken damageToken, $"Expecting linking to result in a single token of the {nameof(DamageLinkedToken)} type.");

            // ACT

            ExecutionAnswerSet answers = new ExecutionAnswerSet(new EffectTargetExecutionAnswer(linkedTokenSet.GetQuestions()[0], targetingEntity));
            Assert.True(GameStateDeltaMaker.TryCreateDelta(linkedTokenSet, answers, gameState, out GameStateDelta generatedDelta), "Should be able to create a game state delta from provided context.");
            GameStateDeltaApplier.ApplyGameStateDelta(gameState, generatedDelta);
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.AreEqual(data.ExpectedEndHealth, gameState.GetNumericQuality(targetingEntity, WellknownQualities.Health), "Expecting absorption to result in expected health total.");
        }
    }
}