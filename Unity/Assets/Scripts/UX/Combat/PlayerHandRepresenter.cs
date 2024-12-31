namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.UX;

    public class PlayerHandRepresenter : MonoBehaviour
    {
        [SerializeReference]
        private GameplayUXController UXController;
        [SerializeReference]
        private CentralGameStateController CentralGameStateControllerInstance;

        [SerializeReference]
        private CombatCardUX CardRepresentationPF;

        [SerializeReference]
        private Transform PlayerHandTransform;
        [SerializeReference]
        private Transform SelectedCardTransform;

        private readonly Dictionary<CardInstance, CombatCardUX> CardsToRepresentations = new Dictionary<CardInstance, CombatCardUX>();

        public DisplayedCardUX SelectedCard { get; set; } = null;

        public void RepresentPlayerHand(IGameStateMutator mutator)
        {
            const float MaximumCardFanDistance = 8f;
            const float CardFanDistance = 1.4f;
            const float FanDegreesMaximum = 30f;
            const int CountForMaximumFanValue = 20;

            const float MaximumDownwardsness = .6f;

            // Delete cards that are in this representer, but not in the hand
            foreach (CardInstance key in new List<CardInstance>(this.CardsToRepresentations.Keys))
            {
                if (this.CardsToRepresentations[key].gameObject.activeInHierarchy && mutator.GetCardZone(key) != WellknownZones.Hand)
                {
                    this.CardsToRepresentations[key].gameObject.SetActive(false);
                }
            }

            // Identify the angle to fan things out
            // Cards in the center are less rotated than cards on the ends
            IReadOnlyList<CardInstance> cardsInHand = mutator.GetCardsInZone(WellknownZones.Hand);
            int numberOfCardsInHand = cardsInHand.Count;

            if (numberOfCardsInHand == 0)
            {
                return;
            }

            float cardsMinusOne = (float)(numberOfCardsInHand - 1);

            float modifiedCardFanDistance = numberOfCardsInHand > 1 ? Mathf.Min(CardFanDistance * (float)numberOfCardsInHand, MaximumCardFanDistance) / cardsMinusOne : 0;
            float leftStartingPoint = -modifiedCardFanDistance * (cardsInHand.Count - 1) / 2f;
            float maxFanAngle = numberOfCardsInHand > CountForMaximumFanValue ? FanDegreesMaximum : Mathf.Lerp(0, FanDegreesMaximum, cardsMinusOne / (float)CountForMaximumFanValue);
            float fanAnglePerIndex = numberOfCardsInHand > 1 ? maxFanAngle / cardsMinusOne * 2f : 0;
            float leftStartingPointAngle = -maxFanAngle;

            for (int ii = 0; ii < numberOfCardsInHand; ii++)
            {
                // Push the cards in the right side of the hand slightly back, so that the edge with the elments on the right overlays properly
                // forward is away from the camera, so it's "further back"
                Vector3 backpush = ii * Vector3.forward * .005f;

                // Identify the angle that this should be at
                float thisCardAngle = leftStartingPointAngle + ii * fanAnglePerIndex;

                // Push the cards that are rotated down so they look more like a fan
                Vector3 downpush = Vector3.down * Mathf.InverseLerp(0, Mathf.Sin(Mathf.Deg2Rad * FanDegreesMaximum), Mathf.Sin(Mathf.Deg2Rad * Mathf.Abs(thisCardAngle))) * MaximumDownwardsness;

                // And where it should be positioned
                Vector3 objectOffset = new Vector3(leftStartingPoint, 0, 0) + new Vector3(modifiedCardFanDistance, 0, 0) * ii + backpush + downpush;

                CombatCardUX newCard = GetUX(cardsInHand[ii]);

                if (ReferenceEquals(this.SelectedCard, newCard))
                {
                    objectOffset += Vector3.up * 1f;
                    newCard.transform.parent = this.SelectedCardTransform;
                }
                else
                {
                    newCard.transform.parent = this.PlayerHandTransform;
                }

                newCard.SetTargetPosition(newCard.transform.parent.position + objectOffset, -thisCardAngle);

                // Does the player meet the requirements of at least one of the effects?
                // TODO: Determine if any requirements are met
                newCard.RequirementsAreMet = true;
            }
        }

        public void Annihilate()
        {
            foreach (CombatCardUX ux in new List<CombatCardUX>(this.CardsToRepresentations.Values))
            {
                Destroy(ux.gameObject);
            }

            this.CardsToRepresentations.Clear();
        }

        public void SelectCurrentCard(DisplayedCardUX selectedCard)
        {
            this.SelectedCard = selectedCard;
            this.SelectedCard.SetFromCard(this.SelectedCard.RepresentedCard, SelectCurrentCard);
            this.UXController.SelectCurrentCard(selectedCard);
            this.RepresentPlayerHand(this.CentralGameStateControllerInstance.GameplayState);
        }

        public void DeselectSelectedCard()
        {
            if (this.SelectedCard != null)
            {
                this.SelectedCard.SetFromCard(this.SelectedCard.RepresentedCard, SelectCurrentCard);
                this.SelectedCard = null;
                this.RepresentPlayerHand(this.CentralGameStateControllerInstance.GameplayState);
            }
        }

        public CombatCardUX GetUX(CardInstance forCard)
        {
            bool wasNotVisibleOrJustCreated = false;

            if (!this.CardsToRepresentations.TryGetValue(forCard, out CombatCardUX representingCard))
            {
                representingCard = Instantiate(this.CardRepresentationPF, this.PlayerHandTransform);
                representingCard.SetFromCard(forCard, SelectCurrentCard);
                this.CardsToRepresentations.Add(forCard, representingCard);
                wasNotVisibleOrJustCreated = true;
            }
            else if (!this.CardsToRepresentations[forCard].isActiveAndEnabled)
            {
                representingCard.gameObject.SetActive(true);
                representingCard.SetFromCard(forCard, SelectCurrentCard);
                wasNotVisibleOrJustCreated = true;
            }

            if (wasNotVisibleOrJustCreated)
            {
                representingCard.SnapToPosition(this.PlayerHandTransform.position);
            }

            if (ReferenceEquals(this.SelectedCard, representingCard))
            {
                this.SelectedCard.SetFromCard(forCard, SelectCurrentCard);
            }

            return representingCard;
        }
    }
}