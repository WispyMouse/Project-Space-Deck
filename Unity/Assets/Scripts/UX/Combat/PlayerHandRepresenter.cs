namespace SFDDCards.UX
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.UX;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;

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

        [Obsolete("Transition to " + nameof(_CardsToRepresentations))]
        private Dictionary<Card, CombatCardUX> CardsToRepresentations = new Dictionary<Card, CombatCardUX>();
        private readonly Dictionary<CardInstance, CombatCardUX> _CardsToRepresentations = new Dictionary<CardInstance, CombatCardUX>();

        public ReactionWindowContext? ReactionWindowForSelectedCard
        {
            get
            {
                return this.reactionWindowForSelectedCard;
            }
            set
            {
                this.reactionWindowForSelectedCard = value;

                if (this.SelectedCard != null)
                {
                    this.SelectedCard.SetFromCard(this.SelectedCard.RepresentedCard, SelectCurrentCard, this.ReactionWindowForSelectedCard);
                }
            }
        }

        private ReactionWindowContext? reactionWindowForSelectedCard { get; set; } = null;

        public DisplayedCardUX SelectedCard { get; set; } = null;

        private void OnEnable()
        {
            GlobalUpdateUX.UpdateUXEvent.AddListener(RepresentPlayerHand);
        }

        private void OnDisable()
        {
            GlobalUpdateUX.UpdateUXEvent.RemoveListener(RepresentPlayerHand);
        }

        [Obsolete("Transition to " + nameof(_RepresentPlayerHand))]
        public void RepresentPlayerHand(CampaignContext forContext)
        {
            const float MaximumCardFanDistance = 8f;
            const float CardFanDistance = 1.4f;
            const float FanDegreesMaximum = 30f;
            const int CountForMaximumFanValue = 20;

            const float MaximumDownwardsness = .6f;

            if (forContext == null)
            {
                return;
            }

            if (forContext.CurrentCombatContext == null)
            {
                this.Annihilate();
                return;
            }

            // Delete cards that are in this representer, but not in the hand
            foreach (Card key in new List<Card>(this.CardsToRepresentations.Keys))
            {
                if (this.CardsToRepresentations[key].gameObject.activeInHierarchy && !forContext.CurrentCombatContext.PlayerCombatDeck.CardsCurrentlyInHand.Contains(key))
                {
                    this.CardsToRepresentations[key].gameObject.SetActive(false);
                }
            }

            // Identify the angle to fan things out
            // Cards in the center are less rotated than cards on the ends
            int cardsInHand = forContext.CurrentCombatContext.PlayerCombatDeck.CardsCurrentlyInHand.Count;

            if (cardsInHand == 0)
            {
                return;
            }

            float cardsMinusOne = (float)(cardsInHand - 1);

            float modifiedCardFanDistance = cardsInHand > 1 ? Mathf.Min(CardFanDistance * (float)cardsInHand, MaximumCardFanDistance) / cardsMinusOne : 0;
            float leftStartingPoint = -modifiedCardFanDistance * (forContext.CurrentCombatContext.PlayerCombatDeck.CardsCurrentlyInHand.Count - 1) / 2f;
            float maxFanAngle = cardsInHand > CountForMaximumFanValue ? FanDegreesMaximum : Mathf.Lerp(0, FanDegreesMaximum, cardsMinusOne / (float)CountForMaximumFanValue);
            float fanAnglePerIndex = cardsInHand > 1 ? maxFanAngle / cardsMinusOne * 2f : 0;
            float leftStartingPointAngle = -maxFanAngle;

            for (int ii = 0; ii < cardsInHand; ii++)
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

                CombatCardUX newCard = GetUX(forContext.CurrentCombatContext.PlayerCombatDeck.CardsCurrentlyInHand[ii]);

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
                bool anyPassingRequirements = ScriptTokenEvaluator.MeetsAnyRequirements(
                    ScriptTokenEvaluator.CalculateConceptualBuildersFromTokenEvaluation(newCard.RepresentedCard),
                    forContext, 
                    newCard.RepresentedCard,
                    forContext.CampaignPlayer,
                    null);
                newCard.RequirementsAreMet = anyPassingRequirements;
            }
        }

        public void _RepresentPlayerHand(CampaignContext forContext)
        {
            const float MaximumCardFanDistance = 8f;
            const float CardFanDistance = 1.4f;
            const float FanDegreesMaximum = 30f;
            const int CountForMaximumFanValue = 20;

            const float MaximumDownwardsness = .6f;

            if (forContext == null)
            {
                return;
            }

            if (forContext.CurrentCombatContext == null)
            {
                this.Annihilate();
                return;
            }

            // Delete cards that are in this representer, but not in the hand
            foreach (CardInstance key in new List<CardInstance>(this._CardsToRepresentations.Keys))
            {
                if (this._CardsToRepresentations[key].gameObject.activeInHierarchy && forContext._CurrentEncounter.GetCardZone(key) != WellknownZones.Hand)
                {
                    this._CardsToRepresentations[key].gameObject.SetActive(false);
                }
            }

            // Identify the angle to fan things out
            // Cards in the center are less rotated than cards on the ends
            IReadOnlyList<CardInstance> cardsInHand = forContext._CurrentEncounter.GetZoneCards(WellknownZones.Hand); ;
            int numberOfCardsInHand = cardsInHand.Count;

            if (numberOfCardsInHand == 0)
            {
                return;
            }

            float cardsMinusOne = (float)(numberOfCardsInHand - 1);

            float modifiedCardFanDistance = numberOfCardsInHand > 1 ? Mathf.Min(CardFanDistance * (float)numberOfCardsInHand, MaximumCardFanDistance) / cardsMinusOne : 0;
            float leftStartingPoint = -modifiedCardFanDistance * (forContext.CurrentCombatContext.PlayerCombatDeck.CardsCurrentlyInHand.Count - 1) / 2f;
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

                CombatCardUX newCard = _GetUX(cardsInHand[ii]);

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

            foreach (CombatCardUX ux in new List<CombatCardUX>(this._CardsToRepresentations.Values))
            {
                Destroy(ux.gameObject);
            }

            this.CardsToRepresentations.Clear();
            this._CardsToRepresentations.Clear();
        }

        [Obsolete("Transition to " + nameof(_SelectCurrentCard))]
        public void SelectCurrentCard(DisplayedCardUX selectedCard)
        {
            this.SelectedCard = selectedCard;
            this.SelectedCard.SetFromCard(this.SelectedCard.RepresentedCard, SelectCurrentCard, GetReactionWindowContextForCard(selectedCard));
            this.UXController.SelectCurrentCard(selectedCard);
            this.RepresentPlayerHand(this.CentralGameStateControllerInstance.CurrentCampaignContext);
        }

        public void _SelectCurrentCard(DisplayedCardUX selectedCard)
        {
            this.SelectedCard = selectedCard;
            this.SelectedCard._SetFromCard(this.SelectedCard._RepresentedCard, _SelectCurrentCard);
            this.UXController.SelectCurrentCard(selectedCard);
            this._RepresentPlayerHand(this.CentralGameStateControllerInstance.CurrentCampaignContext);
        }

        [Obsolete("Transition to " + nameof(_DeselectSelectedCard))]
        public void DeselectSelectedCard()
        {
            if (this.SelectedCard != null)
            {
                this.SelectedCard.SetFromCard(this.SelectedCard.RepresentedCard, SelectCurrentCard, null);
                this.SelectedCard = null;
                this.RepresentPlayerHand(this.CentralGameStateControllerInstance.CurrentCampaignContext);
            }
        }

        public void _DeselectSelectedCard()
        {
            if (this.SelectedCard != null)
            {
                this.SelectedCard._SetFromCard(this.SelectedCard._RepresentedCard, _SelectCurrentCard);
                this.SelectedCard = null;
                this._RepresentPlayerHand(this.CentralGameStateControllerInstance.CurrentCampaignContext);
            }
        }

        [Obsolete("Transition to " + nameof(_GetUX))]
        public CombatCardUX GetUX(Card forCard)
        {
            bool wasNotVisibleOrJustCreated = false;

            if (!this.CardsToRepresentations.TryGetValue(forCard, out CombatCardUX representingCard))
            {
                representingCard = Instantiate(this.CardRepresentationPF, this.PlayerHandTransform);
                representingCard.SetFromCard(forCard, SelectCurrentCard, null);
                this.CardsToRepresentations.Add(forCard, representingCard);
                wasNotVisibleOrJustCreated = true;
            }
            else if (!this.CardsToRepresentations[forCard].isActiveAndEnabled)
            {
                representingCard.gameObject.SetActive(true);
                representingCard.SetFromCard(forCard, SelectCurrentCard, null);
                wasNotVisibleOrJustCreated = true;
            }

            if (wasNotVisibleOrJustCreated)
            {
                representingCard.SnapToPosition(this.PlayerHandTransform.position);
            }

            if (ReferenceEquals(this.SelectedCard, representingCard))
            {
                this.SelectedCard.SetFromCard(forCard, SelectCurrentCard, this.ReactionWindowForSelectedCard);
            }

            return representingCard;
        }

        public CombatCardUX _GetUX(CardInstance forCard)
        {
            bool wasNotVisibleOrJustCreated = false;

            if (!this._CardsToRepresentations.TryGetValue(forCard, out CombatCardUX representingCard))
            {
                representingCard = Instantiate(this.CardRepresentationPF, this.PlayerHandTransform);
                representingCard._SetFromCard(forCard, _SelectCurrentCard);
                this._CardsToRepresentations.Add(forCard, representingCard);
                wasNotVisibleOrJustCreated = true;
            }
            else if (!this._CardsToRepresentations[forCard].isActiveAndEnabled)
            {
                representingCard.gameObject.SetActive(true);
                representingCard._SetFromCard(forCard, _SelectCurrentCard);
                wasNotVisibleOrJustCreated = true;
            }

            if (wasNotVisibleOrJustCreated)
            {
                representingCard.SnapToPosition(this.PlayerHandTransform.position);
            }

            if (ReferenceEquals(this.SelectedCard, representingCard))
            {
                this.SelectedCard._SetFromCard(forCard, _SelectCurrentCard);
            }

            return representingCard;
        }

        [Obsolete("ReactionWindowContext is from the old namespaces and codebase. Should be replaced with parity.")]
        public ReactionWindowContext? GetReactionWindowContextForCard(DisplayedCardUX ux)
        {
            if (this.CentralGameStateControllerInstance.CurrentCampaignContext.CurrentCombatContext == null)
            {
                return null;
            }

            return new ReactionWindowContext()
            {
                CampaignContext = this.CentralGameStateControllerInstance.CurrentCampaignContext,
                CombatantEffectOwner = this.CentralGameStateControllerInstance.CurrentCampaignContext.CampaignPlayer,
                CombatantTarget = UXController.HoveredCombatant,
                PlayedFromZone = "hand",
                TimingWindowId = KnownReactionWindows.ConsideringPlayingFromHand
            };
        }
    }
}