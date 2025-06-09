using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using SpaceDeck.GameState.Minimum;
using SpaceDeck.GameState.Rules;
using SpaceDeck.Models.Databases;
using SpaceDeck.Models.Imports;
using SpaceDeck.Tokenization.Evaluatables;
using SpaceDeck.Tokenization.Functions;
using SpaceDeck.Tokenization.Processing;
using SpaceDeck.Tokenization.ScriptingCommands;
using SpaceDeck.UX.AssetLookup;
using UnityEngine;

namespace SpaceDeck.UX
{
    public class DefaultLoader : MonoBehaviour
    {
        private static List<Action> callsAfterLoaded = new List<Action>();
        [SerializeReference]
        private List<GameObject> ObjectsToAwakenAfterLoading = new List<GameObject>();

        public static bool DefaultsLoaded = false;

        [SerializeReference]
        private TextMeshProSpriteController TextMeshProSpriteControllerInstance;

        private void Awake()
        {
            if (DefaultsLoaded)
            {
                this.AwakenSleepingNodes();
                this.CallAndClearQueue();

                return;
            }

            foreach (GameObject objectToHideBeforeLoading in this.ObjectsToAwakenAfterLoading)
            {
                objectToHideBeforeLoading.SetActive(false);
            }

            // EVALUATABLE PARSERS
            // TODO: Custom parsers? Bring in from other dlls?
            EvaluatablesReference.SubscribeEvaluatable(new CompositeNumericEvaluatableParser());
            EvaluatablesReference.SubscribeEvaluatable(new ConstantNumericEvaluatableParser());
            EvaluatablesReference.SubscribeEvaluatable(new SelfTargetEvaluatableParser());
            EvaluatablesReference.SubscribeEvaluatable(new FoeTargetEvaluatableParser());

            // FUNCTION PARSERS
            EvaluatablesReference.SubscribeEvaluatable(new CountCurrencyEvaluatableParser());
            EvaluatablesReference.SubscribeEvaluatable(new CountElementEvaluatableParser());
            EvaluatablesReference.SubscribeEvaluatable(new CountStacksEvaluatableParser());

            // RULES
            // TODO: Rules should be defined and then brought in, rather than prescribed.
            // For now, just import the same set of rules always.
            RuleReference.RegisterRule(new EncounterStartCopyDeckRule());
            RuleReference.RegisterRule(new EncounterStartPlayerTurnRule());
            RuleReference.RegisterRule(new FactionEndTurnNextFactionRule());
            RuleReference.RegisterRule(new FactionStartsFirstTurnRule());
            RuleReference.RegisterRule(new MovePlayedCardToDestinationRule());
            RuleReference.RegisterRule(new PlayedCardsAreDiscardedRule());
            RuleReference.RegisterRule(new PlayerTurnStartDrawCardsRule(new ConstantNumericValue(7))); // TODO Variable card draw
            RuleReference.RegisterRule(new TurnEndNextAllyOrEndFactionTurnRule());
            RuleReference.RegisterRule(new ZeroHealthRule());
            RuleReference.RegisterRule(new EntityPicksIntentEncounterStartRule());
            RuleReference.RegisterRule(new EntityPicksIntentTurnEndedRule());
            RuleReference.RegisterRule(new NonPlayerEntityEndsTurnRule());
            RuleReference.RegisterRule(new EntityActsOnIntentRule());
            RuleReference.RegisterRule(new NonAlignedEntitiesAreEnemiesRule());
            RuleReference.RegisterRule(new PlayerTurnEndDiscardRule());
            RuleReference.RegisterRule(new PlayerRemovedGameOverRule());
            RuleReference.RegisterRule(new NoEnemiesGameWonRule());

            // SCRIPTING COMMANDS
            // TODO: Custom scripting commands?
            ScriptingCommandReference.RegisterScriptingCommand(new ApplyStatusEffectStacksScriptingCommand());
            ScriptingCommandReference.RegisterScriptingCommand(new DamageScriptingCommand());
            ScriptingCommandReference.RegisterScriptingCommand(new ModCurrencyScriptingCommand());
            ScriptingCommandReference.RegisterScriptingCommand(new ModifyElementScriptingCommand());
            ScriptingCommandReference.RegisterScriptingCommand(new ReduceIntensityScriptingCommand());
            ScriptingCommandReference.RegisterScriptingCommand(new SetDestinationScriptingCommand());
            ScriptingCommandReference.RegisterScriptingCommand(new TargetScriptingCommand());

            StartCoroutine(BootupSequence());
        }

        public static void CallWhenLoaded(Action toCall)
        {
            if (DefaultsLoaded)
            {
                toCall?.Invoke();
            }
            else
            {
                callsAfterLoaded.Add(toCall);
            }
        }

        private void AwakenSleepingNodes()
        {
            foreach (GameObject objectToAwaken in this.ObjectsToAwakenAfterLoading)
            {
                objectToAwaken.gameObject.SetActive(true);
            }
        }

        IEnumerator BootupSequence()
        {
            SynchronizationContext currentContext = SynchronizationContext.Current;

            yield return ImportHelper.YieldForTask(ImportHelper.ImportImportableFilesIntoDatabaseAsync<CurrencyImport>(Application.streamingAssetsPath, "currencyImport", CurrencyDatabase.AddCurrencyToDatabase, currentContext));
            foreach (Currency currency in CurrencyDatabase.CurrencyData.Values)
            {
                if (SpriteLookup.TryGetSprite(currency.Id, out Sprite foundSprite))
                {
                    int spriteIndex = this.TextMeshProSpriteControllerInstance.AddSprite(foundSprite);
                    SpriteLookup.SetSpriteIndex(currency.Id, spriteIndex);
                }
            }

            yield return ImportHelper.YieldForTask(ImportHelper.ImportImportableFilesIntoDatabaseAsync<ElementImport>(Application.streamingAssetsPath, "elementImport", ElementDatabase.AddElement, currentContext));
            foreach (SpaceDeck.GameState.Minimum.Element element in ElementDatabase.ElementData.Values)
            {
                if (SpriteLookup.TryGetSprite(element.Id, out Sprite foundSprite))
                {
                    int spriteIndex = this.TextMeshProSpriteControllerInstance.AddSprite(foundSprite);
                    SpriteLookup.SetSpriteIndex(element.Id, spriteIndex);
                }
            }

            yield return ImportHelper.YieldForTask(ImportHelper.ImportImportableFilesIntoDatabaseAsync<CardImport>(Application.streamingAssetsPath, "cardImport", CardDatabase.AddCardToDatabase, currentContext));
            yield return ImportHelper.YieldForTask(ImportHelper.ImportImportableFilesIntoDatabaseAsync<StatusEffectImport>(Application.streamingAssetsPath, "statusImport", StatusEffectDatabase.RegisterStatusEffect, currentContext));
            yield return ImportHelper.YieldForTask(ImportHelper.ImportImportableFilesIntoDatabaseAsync<PickRewardImport>(Application.streamingAssetsPath, "rewardImport", RewardDatabase.AddPickReward, currentContext));
            yield return ImportHelper.YieldForTask(ImportHelper.ImportImportableFilesIntoDatabaseAsync<EnemyImport>(Application.streamingAssetsPath, "enemyImport", EnemyDatabase.AddEnemy, currentContext));
            yield return ImportHelper.YieldForTask(ImportHelper.ImportImportableFilesIntoDatabaseAsync<EncounterImport>(Application.streamingAssetsPath, "encounterImport", EncounterDatabase.AddEncounter, currentContext));
            yield return ImportHelper.YieldForTask(ImportHelper.ImportImportableFilesIntoDatabaseAsync<RouteImport>(Application.streamingAssetsPath, "routeImport", RouteDatabase.AddRouteToDatabase, currentContext));

            AllDatabases.LinkAllDatabase();

            DefaultsLoaded = true;
            this.AwakenSleepingNodes();
            this.CallAndClearQueue();
        }

        private void CallAndClearQueue()
        {
            foreach (Action callAfterLoaded in new List<Action>(callsAfterLoaded))
            {
                callAfterLoaded.Invoke();
            }

            callsAfterLoaded.Clear();
        }
    }
}