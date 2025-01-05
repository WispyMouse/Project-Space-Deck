namespace SpaceDeck.Tests.EditMode.Parsing
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
    using SpaceDeck.Tests.EditMode.Common.TestFixtures;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Models.Imports;

    public class ParsingLanguageTests
    {
        [TearDown]
        public void TearDown()
        {
            CommonTestUtility.TearDownDatabases();
        }

        [SetUp]
        public virtual void SetUp()
        {
            StatusEffectDatabase.RegisterStatusEffectPrototype(DebugStatusEffectPrototype);
            ElementDatabase.AddElement(DebugElementImport);
        }

        protected readonly static ElementImport DebugElementImport = new ElementImport() { Id = nameof(DebugElementImport), Name = nameof(DebugElementImport) };
        protected readonly static StatusEffectPrototype DebugStatusEffectPrototype = new StatusEffectPrototype(nameof(DebugStatusEffectPrototype), nameof(DebugStatusEffectPrototype));

        public class AssertScriptParsing_ValueSource_Object
        {
            public string EffectScript;
            public string ExpectedParseValue;

            public AssertScriptParsing_ValueSource_Object(string effectScript, string expectedParseValue)
            {
                this.EffectScript = effectScript;
                this.ExpectedParseValue = expectedParseValue;
            }
        }

        public List<AssertScriptParsing_ValueSource_Object> AssertScriptParsing_ValueSources = new List<AssertScriptParsing_ValueSource_Object>()
        {
            new AssertScriptParsing_ValueSource_Object("[SETTARGET: FOE][DAMAGE: 1]", "1 damage."),
            new AssertScriptParsing_ValueSource_Object("[SETTARGET: SELF][DAMAGE: 1]", "1 damage to self."),

            new AssertScriptParsing_ValueSource_Object("[SETTARGET: SELF][HEAL: 1]", "Heal 1."),
            new AssertScriptParsing_ValueSource_Object("[SETTARGET: FOE][HEAL: 1]", "Heal foe for 1."),

            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][APPLYSTATUSEFFECTSTACKS: 1 {DebugStatusEffectPrototype.Id}]", $"Apply 1 {DebugStatusEffectPrototype.Name} to foe."),
            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: SELF][APPLYSTATUSEFFECTSTACKS: 2 {DebugStatusEffectPrototype.Id}]", $"Gain 2 {DebugStatusEffectPrototype.Name}."),

            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][REMOVESTATUSEFFECTSTACKS: 1 {DebugStatusEffectPrototype.Id}]", $"Remove 1 stack of {DebugStatusEffectPrototype.Name} from foe."),
            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: SELF][REMOVESTATUSEFFECTSTACKS: 2 {DebugStatusEffectPrototype.Id}]", $"Remove 2 stacks of {DebugStatusEffectPrototype.Name} from self."),

            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][SETSTATUSEFFECTSTACKS: 1 {DebugStatusEffectPrototype.Id}]", $"Set {DebugStatusEffectPrototype.Name} to 1 stack on foe."),
            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: SELF][SETSTATUSEFFECTSTACKS: 2 {DebugStatusEffectPrototype.Id}]", $"Set {DebugStatusEffectPrototype.Name} to 2 stacks on self."),

            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][SETELEMENT: 1 {DebugElementImport.Id}]", $"Set {DebugElementImport.Name} to 1."),
            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: SELF][SETELEMENT: 0 {DebugElementImport.Id}]", $"Clear {DebugElementImport.Name}."),

            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][DAMAGE: COUNTSTACKS_{DebugStatusEffectPrototype.Id}]", $"1 x {DebugStatusEffectPrototype.Name} damage."),

            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][REQUIRESATLEASTELEMENT: 2 {DebugElementImport.Id}][DAMAGE: 3]", $"If {DebugElementImport.Name} {CommonTestUtility.GreaterThanOrEqualToAscii} 2: 3 damage."),
            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][REQUIRESATLEASTELEMENT: 2 {DebugElementImport.Id}][REQUIRESATLEASTELEMENT: 5 DEBUGELEMENTTWOID][DAMAGE: 5][SETTARGET: SELF][HEAL: 7]", $"If {DebugElementImport.Name} {CommonTestUtility.GreaterThanOrEqualToAscii} 2, {DebugElementImport.Name} {CommonTestUtility.GreaterThanOrEqualToAscii} 5: 5 damage. Heal self for 7."),

            new AssertScriptParsing_ValueSource_Object($"[DRAW: 1][CARDTARGET: HAND][CHOOSECARDS: 1][MOVECARDTOZONE: DISCARD]", "Draw a card. Discard a card."),
            new AssertScriptParsing_ValueSource_Object($"[DRAW: 2][CARDTARGET: HAND][CHOOSECARDS: 2][MOVECARDTOZONE: DISCARD]", "Draw 2 cards. Discard 2 cards."),

            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][DAMAGE: 1][MOVECARDTOZONE: EXILE]", "1 damage. Exile this card."),

            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][DAMAGE: 1+1]", "2 damage."),
            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][DAMAGE: 2*3]", "6 damage."),
            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][DAMAGE: 9/3]", "3 damage."),
            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][DAMAGE: 4-3]", "1 damage."),

            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][DAMAGE: 1+COUNT({DebugElementImport.Id})]", $"1 + {DebugElementImport.Name} damage."),
            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][DAMAGE: 2*COUNT({DebugElementImport.Id})]", $"2 x {DebugElementImport.Name} damage."),
            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][DAMAGE: 3/COUNT({DebugElementImport.Id})]", $"3 / {DebugElementImport.Name} damage."),
            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][DAMAGE: 4-COUNT({DebugElementImport.Id})]", $"4 - {DebugElementImport.Name} damage."),

            new AssertScriptParsing_ValueSource_Object($"[SETTARGET: FOE][DAMAGE: 1~100]", $"1 ~ 100 damage.")
        };

        [Test]
        [TestCaseSource(nameof(AssertScriptParsing_ValueSources))]
        public void RunParsingTests(AssertScriptParsing_ValueSource_Object dataSource)
        {
            Assert.True(TokenTextMaker.TryGetTokenTextFromString(dataSource.EffectScript, out TokenText tokenText), "Should be able to parse token text.");
            Assert.True(ParsedTokenMaker.TryGetParsedTokensFromTokenText(tokenText, out ParsedTokenList parsedSet), "Should be able to parse tokens.");
            Assert.True(LinkedTokenMaker.TryGetLinkedTokenList(parsedSet, out LinkedTokenList linkedTokens), "Should be able to link tokens.");
            string description = linkedTokens.Describe();
        }
    }
}