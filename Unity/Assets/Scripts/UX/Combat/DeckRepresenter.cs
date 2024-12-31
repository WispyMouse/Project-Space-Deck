namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.GameState.Minimum;

    public class DeckRepresenter : MonoBehaviour
    {
        [SerializeReference]
        private TMPro.TMP_Text CardsInDeckValue;
        [SerializeReference]
        private TMPro.TMP_Text CardsInDiscardValue;
        [SerializeReference]
        private TMPro.TMP_Text CardsInExileValue;

        public void _RepresentDeck(IGameStateMutator forContext)
        {
            if (forContext == null)
            {
                this.CardsInDeckValue.text = "0";
                this.CardsInDiscardValue.text = "0";
                this.CardsInExileValue.text = "0";

                return;
            }

            this.CardsInDeckValue.text = forContext.GetCardsInZone(WellknownZones.Deck).Count.ToString();
            this.CardsInDiscardValue.text = forContext.GetCardsInZone(WellknownZones.Discard).Count.ToString();
            this.CardsInExileValue.text = forContext.GetCardsInZone(WellknownZones.Exile).Count.ToString();
        }
    }
}