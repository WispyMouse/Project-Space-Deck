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
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.Tests.EditMode.Common.TestFixtures;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.GameState.Deltas;
    using SpaceDeck.Tokenization.Functions;
    using SpaceDeck.Utility.Unity;

    /// <summary>
    /// This class holds tests that were made as part of a
    /// test-driven development process of the Execution Library.
    /// 
    /// Its function serves both as a test, and a working diary
    /// of the features being considered and implemented.
    /// </summary>
    public class ExecutionStartTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DebugLogger.SubscribeDebugListener(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            DebugLogger.UnsubscribeDebugListener();
        }

        [TearDown]
        public void TearDown()
        {
            CommonTestUtility.TearDownDatabases();
        }

        /// <summary>
        /// This creates an effect that will result in
        /// logging out a simple message in the Debug logs.
        /// </summary>
        [Test]
        public void DebugLog_NoArguments_AsExpected()
        {
            ScriptingCommandReference.RegisterScriptingCommand(new ZeroArgumentDebugLogScriptingCommand());

            string zeroArgumentDebugLogTokenTextString = $"[{ZeroArgumentDebugLogScriptingCommand.IdentifierString}]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(zeroArgumentDebugLogTokenTextString, out TokenText zeroArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(zeroArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");

            GameState gameState = new GameState();

            Assert.True(GameStateDeltaMaker.TryCreateDelta(linkedTokenSet, gameState, out GameStateDelta generatedDelta), "Should be able to create a game state delta from provided context.");

            Assert.AreEqual(1, generatedDelta.Changes.Count, "Expecting one change.");
            Assert.IsTrue(generatedDelta.Changes[0] is LoggingGameStateChange, "Expecting token to be a logging token.");
            LoggingGameStateChange log = generatedDelta.Changes[0] as LoggingGameStateChange;
            Assert.AreEqual(ZeroArgumentDebugLogScriptingCommand.HelloString, log.ToLog, "Expecting debug log to be as designated.");
        }

        /// <summary>
        /// This creates an effect that will result in
        /// logging out a simple message in the Debug logs.
        /// </summary>
        [Test]
        public void DebugLog_OneArgument_AsExpected()
        {
            ScriptingCommandReference.RegisterScriptingCommand(new OneArgumentDebugLogScriptingCommand());

            string oneArgumentDebugLogTokenTextString = $"[{OneArgumentDebugLogScriptingCommand.IdentifierString}:111]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(oneArgumentDebugLogTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");

            GameState gameState = new GameState();

            Assert.True(GameStateDeltaMaker.TryCreateDelta(linkedTokenSet, gameState, out GameStateDelta generatedDelta), "Should be able to create a game state delta from provided context.");
            Assert.AreEqual(1, generatedDelta.Changes.Count, "Expecting one change.");
            Assert.IsTrue(generatedDelta.Changes[0] is LoggingGameStateChange, "Expecting token to be a logging token.");
            LoggingGameStateChange log = generatedDelta.Changes[0] as LoggingGameStateChange;
            Assert.AreEqual("111", log.ToLog, "Expecting debug log to be as designated.");
        }

        /// <summary>
        /// This test creates a command involving dealing damage.
        /// It doesn't specify a target beforehand, and so it should fail to evaluate.
        /// </summary>
        [Test]
        public void Damage_RequiresTarget_FailWithoutTarget()
        {
            // ARRANGE
            var damageScriptingCommand = new DamageScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(damageScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());

            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            Entity targetingEntity = new Entity();
            targetingEntity.Qualities.SetNumericQuality(WellknownQualities.Health, 100);
            encounter.EncounterEntities.Add(targetingEntity);
            gameState.StartEncounter(encounter);

            // ACT
            string damageArgumentTokenTextString = $"[{damageScriptingCommand.Identifier}:1]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(damageArgumentTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");

            // ASSERT
            // DAMAGE requires a target, and without a target this shouldn't be able to evaluate
            Assert.False(GameStateDeltaMaker.TryCreateDelta(linkedTokenSet, gameState, out GameStateDelta generatedDelta), "Should not be able to create delta without providing context.");
        }

        /// <summary>
        /// This test creates a command involving dealing damage.
        /// The a target is provided programmatically, showing that it can evaluate.
        /// </summary>
        [Test]
        public void Damage_RequiresTarget_SucceedWithTarget()
        {
            // ARRANGE
            var damageScriptingCommand = new DamageScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(damageScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());

            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            Entity targetingEntity = new Entity();
            targetingEntity.Qualities.SetNumericQuality(WellknownQualities.Health, 100);
            encounter.EncounterEntities.Add(targetingEntity);
            gameState.StartEncounter(encounter);

            // ACT
            string damageArgumentTokenTextString = $"[{damageScriptingCommand.Identifier}:1]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(damageArgumentTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");
            ExecutionAnswerSet answers = new ExecutionAnswerSet(new EffectTargetExecutionAnswer(linkedTokenSet.GetQuestions()[0], targetingEntity));

            // ASSERT
            // With the appropriate answers provided, assert this can be performed
            Assert.True(GameStateDeltaMaker.TryCreateDelta(linkedTokenSet, answers, gameState, out GameStateDelta generatedDelta), "Should be able to create delta after providing answers.");
        }

        /// <summary>
        /// This is a test of the system leading up to damage being applied to a character.
        /// Through the flow of parsing, linking, and applying to a game state, we can see if damage can result
        /// in a different game state value being set when the entire operation runs.
        /// 
        /// </summary>
        [Test]
        public void Damage_CausesHealthLoss()
        {
            var damageScriptingCommand = new DamageScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(damageScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());

            string damageArgumentTokenTextString = $"[{damageScriptingCommand.Identifier}:1]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(damageArgumentTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");
            Assert.True(linkedTokenSet.Scopes.Count == 1 && linkedTokenSet.Scopes[0].Tokens.Count == 1 && linkedTokenSet.Scopes[0].Tokens[0] is DamageLinkedToken damageToken, $"Expecting linking to result in a single token of the {nameof(DamageLinkedToken)} type.");

            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            Entity targetingEntity = new Entity();
            targetingEntity.Qualities.SetNumericQuality(WellknownQualities.Health, 100);
            encounter.EncounterEntities.Add(targetingEntity);
            gameState.StartEncounter(encounter);

            ExecutionAnswerSet answers = new ExecutionAnswerSet(new EffectTargetExecutionAnswer(linkedTokenSet.GetQuestions()[0], targetingEntity));
            Assert.True(GameStateDeltaMaker.TryCreateDelta(linkedTokenSet, answers, gameState, out GameStateDelta generatedDelta), "Should be able to create a game state delta from provided context.");

            Assert.AreEqual(1, generatedDelta.Changes.Count, "Expecting one change.");
            Assert.IsTrue(generatedDelta.Changes[0] is ModifyNumericQuality, "Expecting token to be a quality change token.");
            ModifyNumericQuality modifyQuality = generatedDelta.Changes[0] as ModifyNumericQuality;
            Assert.AreEqual(modifyQuality.Intensity, -1, "Expecting damage amount to be (negative) one.");

            GameStateDeltaApplier.ApplyGameStateDelta(gameState, generatedDelta);
            Assert.AreEqual(99, gameState.GetAllEntities()[0].Qualities.GetNumericQuality(WellknownQualities.Health), "Expecting health to currently be 1 less than starting, so 99.");
        }

        /// <summary>
        /// Similar to <see cref="Damage_RequiresTarget_SucceedWithTarget"/>, this aims to succeed.
        /// This one uses the AnswererInterface, so it is a soft test of that tool.
        /// </summary>
        [Test]
        public void Damage_RequiresTarget_ProvidedByAnswererInterface()
        {
            // ARRANGE
            var damageScriptingCommand = new DamageScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(damageScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());
            var targetScriptingCommand = new TargetScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(targetScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new FoeTargetEvaluatableParser());

            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();
            Entity targetingEntity = new Entity();
            targetingEntity.Qualities.SetNumericQuality(WellknownQualities.Health, 100);
            encounter.EncounterEntities.Add(targetingEntity);
            gameState.StartEncounter(encounter);

            // ACT
            string damageArgumentTokenTextString = $"[TARGET:FOE][{damageScriptingCommand.Identifier}:1]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(damageArgumentTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");

            ExecutionAnswerSet answers = null;
            new TestSpecificTargetAnswerer(targetingEntity).HandleQuestions(new QuestionAnsweringContext(gameState, targetingEntity, null), linkedTokenSet.GetQuestions(), (ExecutionAnswerSet handledAnswer) =>
            {
                answers = handledAnswer;
            });

            // ASSERT
            // With the appropriate answers provided, assert this can be performed
            Assert.True(GameStateDeltaMaker.TryCreateDelta(linkedTokenSet, answers, gameState, out GameStateDelta generatedDelta), "Should be able to create delta after providing answers.");
        }

        /// <summary>
        /// Similar to <see cref="Damage_RequiresTarget_SucceedWithTarget"/>, this aims to succeed.
        /// This one uses the <see cref="IndexChoosingAnswerer"/> to pick.
        /// This will show that the targets are enumerated properly in the damage scripting command.
        /// </summary>
        [Test]
        public void Damage_RequiresTarget_ProvidedByIndexChoosingInterface()
        {
            // ARRANGE
            var damageScriptingCommand = new DamageScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(damageScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());
            var targetScriptingCommand = new TargetScriptingCommand();
            ScriptingCommandReference.RegisterScriptingCommand(targetScriptingCommand);
            EvaluatablesReference.SubscribeEvaluatable(new FoeTargetEvaluatableParser());

            GameState gameState = new GameState();
            EncounterState encounter = new EncounterState();

            // Add three enemies, so there's some ambiguity on who to target
            Entity entityOne = new Entity();
            entityOne.Qualities.SetNumericQuality(WellknownQualities.Health, 100);
            entityOne.Qualities.SetNumericQuality(WellknownQualities.Faction, WellknownFactions.Foe);
            encounter.EncounterEntities.Add(entityOne);
            Entity entityTwoThisOneIsTheTarget = new Entity();
            entityTwoThisOneIsTheTarget.Qualities.SetNumericQuality(WellknownQualities.Health, 100);
            entityTwoThisOneIsTheTarget.Qualities.SetNumericQuality(WellknownQualities.Faction, WellknownFactions.Foe);
            encounter.EncounterEntities.Add(entityTwoThisOneIsTheTarget);
            Entity entityThree = new Entity();
            entityThree.Qualities.SetNumericQuality(WellknownQualities.Health, 100);
            entityThree.Qualities.SetNumericQuality(WellknownQualities.Faction, WellknownFactions.Foe);
            encounter.EncounterEntities.Add(entityThree);
            gameState.StartEncounter(encounter);

            // ACT
            string damageArgumentTokenTextString = $"[TARGET:FOE][{damageScriptingCommand.Identifier}:1]";
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(damageArgumentTokenTextString, out TokenText oneArgumentTokenText), "Should be able to parse Token Text String into Token Text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(oneArgumentTokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens from token text.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokenSet), "Should be able to link tokens.");

            ExecutionAnswerSet answers = null;
            new IndexChoosingAnswerer(1).HandleQuestions(new QuestionAnsweringContext(gameState, entityOne, null), linkedTokenSet.GetQuestions(), (ExecutionAnswerSet handledAnswer) =>
            {
                answers = handledAnswer;
            });

            // ASSERT
            // With the appropriate answers provided, assert this can be performed
            Assert.True(GameStateDeltaMaker.TryCreateDelta(linkedTokenSet, answers, gameState, out GameStateDelta generatedDelta), "Should be able to create delta after providing answers.");
            GameStateDeltaApplier.ApplyGameStateDelta(gameState, generatedDelta);

            Assert.AreEqual(100, entityOne.Qualities.GetNumericQuality(WellknownQualities.Health), "The first target should be unharmed.");
            Assert.AreEqual(99, entityTwoThisOneIsTheTarget.Qualities.GetNumericQuality(WellknownQualities.Health), "The second target should be specifically harmed to 99 health.");
            Assert.AreEqual(100, entityThree.Qualities.GetNumericQuality(WellknownQualities.Health), "The third target should be unharmed.");
        }

        /// <summary>
        /// Applies a status effect to an entity, and then tries to evaluate a debug card.
        /// The card should report the correct amount of stacks.
        /// </summary>
        [Test]
        public void CountStacks_CountsSelf_OnlySelf_Correctly()
        {
            decimal reportedValue = 0;
            int numberOfStacks = 10;

            // ARRANGE
            StatusEffectImport import = new StatusEffectImport()
            {
                Id = nameof(CountStacks_CountsSelf_OnlySelf_Correctly)
            };
            StatusEffectDatabase.RegisterStatusEffect(import);
            Action<decimal> setReportedValue = (decimal value) => { reportedValue = value; };

            // TODO: Register evaluatables that will parse token stacks
            INumericEvaluatableValue stacksEvaluatable = null;
            Assert.True(EvaluatablesReference.TryGetNumericEvaluatableValue($"COUNT(SELF,{import.Id})", out stacksEvaluatable), "Should be able to parse the count token.");
            EvaluateAndLogNumericLinkedToken evaluateAndLogNumericLinkedToken = new EvaluateAndLogNumericLinkedToken(stacksEvaluatable, setReportedValue);
            GameState gameState = new GameState();
            EncounterState encounterState = new EncounterState();

            // ACT
            Entity toStack = new Entity();
            encounterState.EncounterEntities.Add(toStack);
            gameState.StartEncounter(encounterState);
            gameState.ModStatusEffectStacks(toStack, import.Id, numberOfStacks);
            gameState.EntityPerformsAction(toStack, evaluateAndLogNumericLinkedToken);

            // ASSERT
            Assert.NotZero(numberOfStacks, "Stacks should have been set by the effect.");
            Assert.AreEqual(numberOfStacks, gameState.GetStacks(toStack, import.Id), "The evaluation should accurately count the stacks.");
        }

        /// <summary>
        /// A card that is played gains elements.
        /// The card then outputs the amount of elements, which should be modified
        /// by the element gain of the card.
        /// </summary>
        [Test]
        public void GainElement_Counts()
        {
            int elementGain = 3;

            // ARRANGE
            ElementImport elementImport = new ElementImport()
            {
                Id = nameof(elementImport)
            };
            ElementDatabase.AddElement(elementImport);
            Element element = ElementDatabase.GetElement(elementImport.Id);
            ScriptingCommandReference.RegisterScriptingCommand(new OneArgumentDebugLogScriptingCommand());
            // TODO: IMPORT SOME EVALUATABLE THAT CAN HANDLE ELEMENTSTACKS
            CardImport cardImport = new CardImport()
            {
                Id = nameof(cardImport),
                EffectScript = $"[{OneArgumentDebugLogScriptingCommand.IdentifierString}:ELEMENTSTACKS({elementImport.Id})]",
                ElementGain = new List<ElementGainImport>() { new ElementGainImport() { ElementId = OneArgumentDebugLogScriptingCommand.IdentifierString, ModAmount = elementGain } }
            };
            CardDatabase.AddCardToDatabase(cardImport);
            CardDatabase.LinkTokens();
            CardInstance instance = CardDatabase.GetInstance(cardImport.Id);
            GameState gameState = new GameState();
            EncounterState encounterState = new EncounterState();
            Entity toStack = new Entity();
            encounterState.EncounterEntities.Add(toStack);

            // ACT
            gameState.StartEncounter(encounterState);
            gameState.StartConsideringPlayingCard(instance);
            gameState.TryExecuteCurrentCard(new ExecutionAnswerSet(toStack));
            

            // ASSERT
            Assert.AreEqual(elementGain.ToString(), LoggingGameStateChange.LastLog, "The last log from the LoggingGameStateChange should be the expected amount of stacks.");
            Assert.AreEqual(elementGain, gameState.Elements[element], "There should be the expected amount of elements present.");
        }

        /// <summary>
        /// Gives the player ten of a currency, then asserts they received it.
        /// </summary>
        [Test]
        public void CountCurrency_Ten()
        {
            // SET PARAMETERS
            int currencyGainAmount = 10;

            // IMPORT DATABASES
            ScriptingCommandReference.RegisterScriptingCommand(new OneArgumentNumericDebugLogScriptingCommand());
            ScriptingCommandReference.RegisterScriptingCommand(new ModCurrencyScriptingCommand());

            EvaluatablesReference.SubscribeEvaluatable(new CountCurrencyEvaluatableParser());
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());

            CurrencyImport toGain = new CurrencyImport()
            {
                Id = nameof(toGain)
            };
            CurrencyDatabase.AddCurrencyToDatabase(toGain);
            Currency gainedCurrency = CurrencyDatabase.Get(toGain.Id);

            CardImport cardImport = new CardImport()
            {
                Id = nameof(cardImport),
                EffectScript = $"[MODCURRENCY:{gainedCurrency.Id} {currencyGainAmount}][{OneArgumentNumericDebugLogScriptingCommand.IdentifierString}:COUNTCURRENCY({gainedCurrency.Id})]"
            };
            CardDatabase.AddCardToDatabase(cardImport);

            AllDatabases.LinkAllDatabase();

            // ARRANGE
            GameState gameState = new GameState();
            EncounterState encounterState = new EncounterState();
            CardInstance instance = CardDatabase.GetInstance(cardImport.Id);

            // ACT
            gameState.StartEncounter(encounterState);
            gameState.StartConsideringPlayingCard(instance);
            Assert.IsTrue(gameState.TryExecuteCurrentCard(new ExecutionAnswerSet(user: null)), "Should be able to execute question-less card.");
            PendingResolveExecutor.ResolveAll(gameState);

            // ASSERT
            Assert.True(gameState.Currencies.TryGetValue(gainedCurrency, out int currentvalue));
            Assert.AreEqual(currencyGainAmount, currentvalue, "Should gain expected currency.");
            Assert.AreEqual(currencyGainAmount.ToString(), LoggingGameStateChange.LastLog, "The last log from the LoggingGameStateChange should be the expected amount of currency.");
        }
    }
}