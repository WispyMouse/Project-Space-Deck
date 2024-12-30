namespace SpaceDeck.UX
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.UX;

    public class PlayCampaignButton : MonoBehaviour
    {
        public Route _RepresentingRoute;

        private CampaignChooserUX ChooserUX;

        public TMPro.TMP_Text Label;

        public void _SetFromRoute(Route route, CampaignChooserUX chooserUX)
        {
            this._RepresentingRoute = route;
            this.ChooserUX = chooserUX;
            this.Label.text = route.Name;
        }

        public void _RouteChosen()
        {
            this.ChooserUX._RouteChosen(this._RepresentingRoute);
        }
    }
}