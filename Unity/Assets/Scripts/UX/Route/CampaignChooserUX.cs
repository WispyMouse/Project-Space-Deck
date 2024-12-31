namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SpaceDeck.GameState.Minimum;

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

        public void RouteChosen(Route chosenRoute)
        {
            this.UXController.RouteChosen(chosenRoute);
        }

        public void ShowChooser()
        {
            this.gameObject.SetActive(true);

            if (!this.Initialized)
            {
                this.Initialized = true;
                foreach (Route route in SpaceDeck.Models.Databases.RouteDatabase.AllRoutes)
                {
                    PlayCampaignButton nextButton = Instantiate(this.SelectorPrefab, ChoiceHolder);
                    nextButton.SetFromRoute(route, this);
                }
            }
        }

        public void HideChooser()
        {
            this.gameObject.SetActive(false);
        }
    }
}