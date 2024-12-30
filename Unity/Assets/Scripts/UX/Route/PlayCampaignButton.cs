namespace SpaceDeck.UX
{
    using SFDDCards;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.UX;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayCampaignButton : MonoBehaviour
    {
        [System.Obsolete("Transition to " + nameof(_RepresentingRoute))]
        public RouteImport RepresentingRoute;
        public Route _RepresentingRoute;

        private CampaignChooserUX ChooserUX;

        public TMPro.TMP_Text Label;

        [System.Obsolete("Transition to " + nameof(_SetFromRoute))]
        public void SetFromRoute(RouteImport route, CampaignChooserUX chooserUX)
        {
            this.RepresentingRoute = route;
            this.ChooserUX = chooserUX;
            this.Label.text = route.RouteName;
        }

        public void _SetFromRoute(Route route, CampaignChooserUX chooserUX)
        {
            this._RepresentingRoute = route;
            this.ChooserUX = chooserUX;
            this.Label.text = route.Name;
        }

        [System.Obsolete("Transition to " + nameof(_RouteChosen))]
        public void RouteChosen()
        {
            this.ChooserUX.RouteChosen(this.RepresentingRoute);
        }

        public void _RouteChosen()
        {
            this.ChooserUX._RouteChosen(this._RepresentingRoute);
        }
    }
}