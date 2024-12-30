namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.UX;
    using SpaceDeck.GameState.Minimum;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CampaignChooserUX : MonoBehaviour
    {
        [SerializeReference]
        private GameplayUXController UXController;

        [SerializeReference]
        private PlayCampaignButton SelectorPrefab;

        [SerializeReference]
        private Transform ChoiceHolder;

        bool Initialized { get; set; } = false;

        private void Start()
        {
            
        }

        [Obsolete("Transition to " + nameof(_RouteChosen))]
        public void RouteChosen(RouteImport chosenRoute)
        {
            this.UXController.RouteChosen(chosenRoute);
        }

        public void _RouteChosen(Route chosenRoute)
        {
            this.UXController._RouteChosen(chosenRoute);
        }

        public void _ShowChooser()
        {
            this.gameObject.SetActive(true);

            if (!this.Initialized)
            {
                this.Initialized = true;
                foreach (Route route in SpaceDeck.Models.Databases.RouteDatabase.AllRoutes)
                {
                    PlayCampaignButton nextButton = Instantiate(this.SelectorPrefab, ChoiceHolder);
                    nextButton._SetFromRoute(route, this);
                }
            }
        }

        public void HideChooser()
        {
            this.gameObject.SetActive(false);
        }
    }
}