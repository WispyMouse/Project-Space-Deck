namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.UX;
    using SpaceDeck.UX.AssetLookup;

    public class CentralGameStateController : MonoBehaviour
    {
        public SpaceDeck.GameState.Execution.GameState GameplayState;
        public Entity CampaignPlayer;

        [SerializeReference]
        private GameplayUXController UXController;

        [SerializeReference]
        private TextMeshProSpriteController TextMeshProSpriteControllerInstance;

        private void Awake()
        {
        }

        void Start()
        {
            this.StartCoroutine(this.BootupSequence());
        }

        /// <summary>
        /// Starts up a new game and begins it.
        /// This will disable all other Controllers, reset all state based information, and generally clean the slate.
        /// Then the game will transition in to a new, playable state.
        /// </summary>
        public void SetupAndStartNewGame()
        {
            this.UXController.Annihilate();
            this.GameplayState = null;
            this.UXController.ShowCampaignChooser();
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
            yield return ImportHelper.YieldForTask(ImportHelper.ImportImportableFilesIntoDatabaseAsync<EnemyImport>(Application.streamingAssetsPath, "enemyImport", EnemyDatabase.AddEnemy, currentContext));
            yield return ImportHelper.YieldForTask(ImportHelper.ImportImportableFilesIntoDatabaseAsync<RewardImport>(Application.streamingAssetsPath, "rewardImport", RewardDatabase.AddReward, currentContext));
            yield return ImportHelper.YieldForTask(ImportHelper.ImportImportableFilesIntoDatabaseAsync<EncounterImport>(Application.streamingAssetsPath, "encounterImport", EncounterDatabase.AddEncounter, currentContext));
            yield return ImportHelper.YieldForTask(ImportHelper.ImportImportableFilesIntoDatabaseAsync<RouteImport>(Application.streamingAssetsPath, "routeImport", RouteDatabase.AddRouteToDatabase, currentContext));

            this.SetupAndStartNewGame();
        }

        public void RouteChosen(Route route)
        {
            this.GameplayState = new GameState.Execution.GameState(route);
            foreach (LowercaseString startingCurrency in route.StartingCurrency.Keys)
            {
                this.GameplayState.ModCurrency(CurrencyDatabase.Get(startingCurrency), route.StartingCurrency[startingCurrency]);
            }
            PlayerUX placedPlayer = this.UXController.PlacePlayerCharacter();
            this.CampaignPlayer = placedPlayer.RepresentedPlayer;
        }
    }
}
