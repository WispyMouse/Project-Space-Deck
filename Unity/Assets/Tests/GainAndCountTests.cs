#if false
namespace SFDDCards.Tests.EditMode
{
    using NUnit.Framework;
    using SFDDCards.Evaluation.Actual;
    using SFDDCards.ImportModels;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Instances;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using UnityEngine;


    public class GainAndCountTests : EditModeTestBase
    {
        /// <summary>
        /// This test is meant to be a test for applying the effect of resource change immediately.
        /// 
        /// Addresses Bug: An ability gains element, and counts it immediately. It should be applied.
        /// </summary>
        [Test]
        public void TestGainAndCount()
        {
            ImportModels.CardImport import = new ImportModels.CardImport()
            {
                Id = nameof(TestGainAndCount),
                Name = nameof(TestGainAndCount),
                ElementGain = new List<ResourceGainImport>()
                {
                    new ResourceGainImport()
                    {
                        Element = DebugElementOne.Id,
                        Gain = 1
                    }
                },
                EffectScript = $"[SETTARGET: FOE][DAMAGE: COUNTELEMENT_{DebugElementOne.Id}]"
            };

            SFDDCards.CardDatabase.AddCardToDatabase(import);
            Card testGainCard = SFDDCards.CardDatabase.GetModel(import.Id);

            EncounterModel testEncounter = EditModeTestCommon.GetEncounterWithPunchingBags(1, 100);
            CampaignContext campaignContext = EditModeTestCommon.GetBlankCampaignContext();
            campaignContext.StartNextRoomFromEncounter(new EvaluatedEncounter(testEncounter));
            CombatContext combatContext = campaignContext.CurrentCombatContext;
            combatContext.EndCurrentTurnAndChangeTurn(CombatContext.TurnStatus.PlayerTurn);
            GlobalSequenceEventHolder.SynchronouslyResolveAllEvents(campaignContext);

            combatContext.PlayerCombatDeck.CardsCurrentlyInHand.Add(testGainCard);
            combatContext.PlayCard(testGainCard, combatContext.Enemies[0]);
            GlobalSequenceEventHolder.SynchronouslyResolveAllEvents(campaignContext);

            Assert.AreEqual(99, combatContext.Enemies[0].CurrentHealth, "The effect should have dealt exactly one damage.");
        }

        /// <summary>
        /// There is no currency with this name.
        /// This should fail to evaluate the currency count token.
        /// There should be no exception, but ignore any logs.
        /// </summary>
        [Test]
        public void CountCurrency_ZeroCurrency()
        {
            ImportModels.CardImport import = new ImportModels.CardImport()
            {
                Id = nameof(CountCurrency_ZeroCurrency),
                Name = nameof(CountCurrency_ZeroCurrency),
                EffectScript = $"[LOGINT: COUNTCURRENCY_CURRENCY_DOESNOTEXIST]"
            };

            SFDDCards.CardDatabase.AddCardToDatabase(import);
            Card testGainCard = SFDDCards.CardDatabase.GetModel(import.Id);

            EncounterModel testEncounter = EditModeTestCommon.GetEncounterWithPunchingBags(1, 100);
            CampaignContext campaignContext = EditModeTestCommon.GetBlankCampaignContext();
            campaignContext.StartNextRoomFromEncounter(new EvaluatedEncounter(testEncounter));
            CombatContext combatContext = campaignContext.CurrentCombatContext;
            combatContext.EndCurrentTurnAndChangeTurn(CombatContext.TurnStatus.PlayerTurn);
            GlobalSequenceEventHolder.SynchronouslyResolveAllEvents(campaignContext);

            UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
            GamestateDelta delta = ScriptTokenEvaluator.CalculateRealizedDeltaEvaluation(testGainCard, campaignContext, combatContext.CombatPlayer, combatContext.Enemies[0]);
            Assert.GreaterOrEqual(delta.DeltaEntries.Count, 1, $"There should be enough delta entries to find the specified delta index. Not enough entries returned on evaluation.");
            Assert.IsFalse(delta.DeltaEntries[0].ConceptualIntensity.TryEvaluateValue(campaignContext, delta.DeltaEntries[0].MadeFromBuilder, out int evaluatedIntensity), "Should evaluate currency, because that currency does not exist.");
            Assert.AreEqual(0, evaluatedIntensity, $"There is no currency with that name. This should not throw an exception, and should return 0.");
        }

        /// <summary>
        /// Set a currency value to 10.
        /// The count currency value should be accurate.
        /// </summary>
        [Test]
        public void CountCurrency_Ten()
        {
            CurrencyDatabase.AddCurrencyToDatabase(new Currency("TESTCURRENCY", "TESTCURRENCY"));

            ImportModels.CardImport import = new ImportModels.CardImport()
            {
                Id = nameof(CountCurrency_ZeroCurrency),
                Name = nameof(CountCurrency_ZeroCurrency),
                EffectScript = $"[LOGINT: COUNTCURRENCY_TESTCURRENCY]"
            };

            SFDDCards.CardDatabase.AddCardToDatabase(import);
            Card testGainCard = SFDDCards.CardDatabase.GetModel(import.Id);

            EncounterModel testEncounter = EditModeTestCommon.GetEncounterWithPunchingBags(1, 100);
            CampaignContext campaignContext = EditModeTestCommon.GetBlankCampaignContext();
            campaignContext.StartNextRoomFromEncounter(new EvaluatedEncounter(testEncounter));

            campaignContext._SetCurrency(CurrencyDatabase.Get("TESTCURRENCY"), 10);

            CombatContext combatContext = campaignContext.CurrentCombatContext;
            combatContext.EndCurrentTurnAndChangeTurn(CombatContext.TurnStatus.PlayerTurn);
            GlobalSequenceEventHolder.SynchronouslyResolveAllEvents(campaignContext);

            GamestateDelta delta = ScriptTokenEvaluator.CalculateRealizedDeltaEvaluation(testGainCard, campaignContext, combatContext.CombatPlayer, combatContext.Enemies[0]);
            Assert.GreaterOrEqual(delta.DeltaEntries.Count, 1, $"There should be enough delta entries to find the specified delta index. Not enough entries returned on evaluation.");
            Assert.IsTrue(delta.DeltaEntries[0].ConceptualIntensity.TryEvaluateValue(campaignContext, delta.DeltaEntries[0].MadeFromBuilder, out int evaluatedIntensity), "Should be able to evaluate intensity.");
            Assert.AreEqual(10, evaluatedIntensity, $"After having the currency set, its value should be 10.");
        }

        /// <summary>
        /// Same as <see cref="CountCurrency_Ten"/>, but instead modify the currency by using a MODCURRENCY token.
        /// </summary>
        [Test]
        public void ModCurrency_Ten()
        {
            CurrencyDatabase.AddCurrencyToDatabase(new Currency("TESTCURRENCY", "TESTCURRENCY"));

            ImportModels.CardImport import = new ImportModels.CardImport()
            {
                Id = nameof(CountCurrency_ZeroCurrency),
                Name = nameof(CountCurrency_ZeroCurrency),
                EffectScript = $"[LOGINT: COUNTCURRENCY_TESTCURRENCY]"
            };

            ImportModels.CardImport modCurrencyImport = new ImportModels.CardImport()
            {
                Id = nameof(modCurrencyImport),
                Name = nameof(modCurrencyImport),
                EffectScript = "[MODCURRENCY: 10 TESTCURRENCY]"
            };

            SFDDCards.CardDatabase.AddCardToDatabase(import);
            Card testGainCard = SFDDCards.CardDatabase.GetModel(import.Id);

            SFDDCards.CardDatabase.AddCardToDatabase(modCurrencyImport);
            Card modCurrencyCard = SFDDCards.CardDatabase.GetModel(modCurrencyImport.Id);

            EncounterModel testEncounter = EditModeTestCommon.GetEncounterWithPunchingBags(1, 100);
            CampaignContext campaignContext = EditModeTestCommon.GetBlankCampaignContext();
            campaignContext.StartNextRoomFromEncounter(new EvaluatedEncounter(testEncounter));
            CombatContext combatContext = campaignContext.CurrentCombatContext;
            combatContext.EndCurrentTurnAndChangeTurn(CombatContext.TurnStatus.PlayerTurn);
            GlobalSequenceEventHolder.SynchronouslyResolveAllEvents(campaignContext);

            combatContext.PlayerCombatDeck.CardsCurrentlyInHand.Add(modCurrencyCard);
            combatContext.PlayCard(modCurrencyCard, combatContext.Enemies[0]);
            GlobalSequenceEventHolder.SynchronouslyResolveAllEvents(campaignContext);

            GamestateDelta delta = ScriptTokenEvaluator.CalculateRealizedDeltaEvaluation(testGainCard, campaignContext, combatContext.CombatPlayer, combatContext.Enemies[0]);
            Assert.GreaterOrEqual(delta.DeltaEntries.Count, 1, $"There should be enough delta entries to find the specified delta index. Not enough entries returned on evaluation.");
            Assert.IsTrue(delta.DeltaEntries[0].ConceptualIntensity.TryEvaluateValue(campaignContext, delta.DeltaEntries[0].MadeFromBuilder, out int evaluatedIntensity), "Should be able to evaluate intensity.");
            Assert.AreEqual(10, evaluatedIntensity, $"After having the currency modified, its value should be 10.");
        }

        /// <summary>
        /// Same as <see cref="CountCurrency_Ten"/>, but instead modify the currency by using a SETCURRENCY token.
        /// </summary>
        [Test]
        public void SetCurrency_Ten()
        {
            CurrencyDatabase.AddCurrencyToDatabase(new Currency("TESTCURRENCY", "TESTCURRENCY"));

            ImportModels.CardImport import = new ImportModels.CardImport()
            {
                Id = nameof(CountCurrency_ZeroCurrency),
                Name = nameof(CountCurrency_ZeroCurrency),
                EffectScript = $"[LOGINT: COUNTCURRENCY_TESTCURRENCY]"
            };

            ImportModels.CardImport modCurrencyImport = new ImportModels.CardImport()
            {
                Id = nameof(modCurrencyImport),
                Name = nameof(modCurrencyImport),
                EffectScript = "[SETCURRENCY: 10 TESTCURRENCY]"
            };

            SFDDCards.CardDatabase.AddCardToDatabase(import);
            Card testGainCard = SFDDCards.CardDatabase.GetModel(import.Id);

            SFDDCards.CardDatabase.AddCardToDatabase(modCurrencyImport);
            Card modCurrencyCard = SFDDCards.CardDatabase.GetModel(modCurrencyImport.Id);

            EncounterModel testEncounter = EditModeTestCommon.GetEncounterWithPunchingBags(1, 100);
            CampaignContext campaignContext = EditModeTestCommon.GetBlankCampaignContext();
            campaignContext.StartNextRoomFromEncounter(new EvaluatedEncounter(testEncounter));
            CombatContext combatContext = campaignContext.CurrentCombatContext;
            combatContext.EndCurrentTurnAndChangeTurn(CombatContext.TurnStatus.PlayerTurn);
            GlobalSequenceEventHolder.SynchronouslyResolveAllEvents(campaignContext);

            combatContext.PlayerCombatDeck.CardsCurrentlyInHand.Add(modCurrencyCard);
            combatContext.PlayCard(modCurrencyCard, combatContext.Enemies[0]);
            GlobalSequenceEventHolder.SynchronouslyResolveAllEvents(campaignContext);

            GamestateDelta delta = ScriptTokenEvaluator.CalculateRealizedDeltaEvaluation(testGainCard, campaignContext, combatContext.CombatPlayer, combatContext.Enemies[0]);
            Assert.GreaterOrEqual(delta.DeltaEntries.Count, 1, $"There should be enough delta entries to find the specified delta index. Not enough entries returned on evaluation.");
            Assert.IsTrue(delta.DeltaEntries[0].ConceptualIntensity.TryEvaluateValue(campaignContext, delta.DeltaEntries[0].MadeFromBuilder, out int evaluatedIntensity), "Should be able to evaluate intensity.");
            Assert.AreEqual(10, evaluatedIntensity, $"After having the currency set, its value should be 10.");
        }
    }
}
#endif