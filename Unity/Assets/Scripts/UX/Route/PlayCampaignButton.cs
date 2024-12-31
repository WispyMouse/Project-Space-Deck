namespace SpaceDeck.UX
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.UX;

    public class PlayCampaignButton : MonoBehaviour
    {
        public Route RepresentingRoute;

        private CampaignChooserUX ChooserUX;

        public TMPro.TMP_Text Label;

        public void SetFromRoute(Route route, CampaignChooserUX chooserUX)
        {
            this.RepresentingRoute = route;
            this.ChooserUX = chooserUX;
            this.Label.text = route.Name;
        }

        public void RouteChosen()
        {
            this.ChooserUX.RouteChosen(this.RepresentingRoute);
        }
    }
}