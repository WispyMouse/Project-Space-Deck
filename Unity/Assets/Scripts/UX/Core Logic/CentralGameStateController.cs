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
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Functions;
    using SpaceDeck.GameState.Rules;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.Tokenization.ScriptingCommands;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.GameState.Execution;
    using PlasticPipe.PlasticProtocol.Messages;

    public class CentralGameStateController : MonoBehaviour
    {
        public delegate void OnRouteChosenDelegate(Route chosenRoute);
        public static event OnRouteChosenDelegate OnRouteChosen;
        public delegate void OnResetGameDelegate();
        public static event OnResetGameDelegate OnResetGame;

        public SpaceDeck.GameState.Execution.GameState GameplayState;
        public Entity CampaignPlayer;

        [SerializeReference]
        private GameplayUXController UXController;

        [SerializeReference]
        private TextMeshProSpriteController TextMeshProSpriteControllerInstance;

        [SerializeReference]
        private List<GameObject> HideUntilBootupComplete = new List<GameObject>();

        public void Start()
        {
            // Will wait to start the UX until the default loader is finished, or immediately if it is already done
            DefaultLoader.CallWhenLoaded(SetupAndStartNewGame);
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

            OnResetGame?.Invoke();
        }

        public void RouteChosen(Route route)
        {
            this.GameplayState = new GameState(route);
            foreach (LowercaseString startingCurrency in route.StartingCurrency.Keys)
            {
                if (!CurrencyDatabase.TryGet(startingCurrency, out Currency foundCurrency))
                {
                    Logging.DebugLog(WellknownLoggingLevels.Error,
                        WellknownLoggingCategories.Route,
                        $"Attempted to get currency that doesn't exist from currency database. Id '{startingCurrency}'.");
                }
                this.GameplayState.ModCurrency(foundCurrency, route.StartingCurrency[startingCurrency]);
            }

            this.CampaignPlayer = new Entity();
            this.CampaignPlayer.Qualities.SetNumericQuality(WellknownQualities.Faction, WellknownFactions.Player);
            this.CampaignPlayer.Qualities.SetNumericQuality(WellknownQualities.MaximumHealth, 100); // TODO VARIABLE MAX HEALTH
            this.CampaignPlayer.Qualities.SetNumericQuality(WellknownQualities.Health, this.CampaignPlayer.Qualities.GetNumericQuality(WellknownQualities.MaximumHealth));
            this.GameplayState.AddPersistentEntity(this.CampaignPlayer);
            PlayerUX placedPlayer = this.UXController.PlacePlayerCharacter();
            this.GameplayState.StartNextRoomFromCampaign(out LinkedChoiceNode nextChoice);

            // Add starting cards to player's deck
            foreach (LowercaseString startingCard in route.StartingCards)
            {
                this.GameplayState.AddCardToCampaignDeck(CardDatabase.GetInstance(startingCard));
            }

            OnRouteChosen?.Invoke(route);

            this.UXController.PresentNextRouteChoice(nextChoice);
        }

        public void MakeChoiceNodeDecision(ChoiceNodeOption choice)
        {
            this.GameplayState.MakeChoiceNodeDecision(choice);

            if (this.GameplayState.CurrentEncounterState != null)
            {
                this.UXController.RepresentCurrentEncounter();
            }

            PendingResolveExecutor.ResolveAll(this.GameplayState);
        }
    }
}
