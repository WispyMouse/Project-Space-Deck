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

        public class Before_LowersValue_DataSource_Object
        {
            public int StartingHealth;
            public int Damage;
            public int Armor;

            public int ExpectedEndHealth;
            public int ExpectedArmor;

            public Before_LowersValue_DataSource_Object(int startingHealth, int damage, int armor)
            {
                this.StartingHealth = startingHealth;
                this.Damage = damage;
                this.Armor = armor;

                this.ExpectedArmor = Mathf.Max(0, armor - damage);
                this.ExpectedEndHealth = startingHealth - Mathf.Max(0, damage - armor);
            }
        }

        public static Before_LowersValue_DataSource_Object[] Before_LowersValue_DataSource =
        {
            new Before_LowersValue_DataSource_Object(100, 10, 5),
            new Before_LowersValue_DataSource_Object(100, 10, 15),
        };

        /// <summary>
        /// This sets up a situation where health would be lowered, but an
        /// attached status effect is going to reduce the intensity first.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(Before_LowersValue_DataSource))]
        public void Before_LowersValue(Before_LowersValue_DataSource_Object dataSource)
        {
            LowercaseString debugStatusId = "DEBUG";
            int startingHealth = dataSource.StartingHealth;
            int damage = dataSource.Damage;
            int armor = dataSource.Armor;

            // ARRANGE
            StatusEffectImport import = new StatusEffectImport()
            {
                Id = nameof(Before_LowersValue)
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
            encounter.EncounterEntities.Add(targetingEntity);
            gameState.ModStatusEffectStacks(targetingEntity, import.Id, armor);
            gameState.SetNumericQuality(targetingEntity, WellknownQualities.Health, startingHealth);

            // ACT
            gameState.StartEncounter(encounter);
            string damageArgumentTokenTextString = $"[{import.Id}:{damage}]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(damageArgumentTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");
            ExecutionAnswerSet answers = new ExecutionAnswerSet(new EffectTargetExecutionAnswer(linkedTokenSet.GetQuestions()[0], targetingEntity));
            Assert.True(GameStateDeltaMaker.TryCreateDelta(linkedTokenSet, answers, gameState, out GameStateDelta generatedDelta), "Should be able to create delta after providing answers.");
            GameStateDeltaApplier.ApplyGameStateDelta(gameState, generatedDelta);
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.AreEqual(dataSource.ExpectedArmor, gameState.GetStacks(targetingEntity, import.Id), "Should have no block remaining after absorbing impact.");
            Assert.AreEqual(dataSource.ExpectedEndHealth, gameState.GetNumericQuality(targetingEntity, WellknownQualities.Health), "Should have a specific amount of health left after the attack.");
        }
    }
}