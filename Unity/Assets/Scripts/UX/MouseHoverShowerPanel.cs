namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.UX;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.UX;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class MouseHoverShowerPanel : MonoBehaviour
    {
        [SerializeReference]
        private RenderedCard OverlayCard;

        [SerializeReference]
        public RectTransform OwnTransform;

        [SerializeReference]
        private PopupPanel PopupPanelPF;

        [SerializeReference]
        private Transform RightPopupPanelHolderTransform;

        [SerializeReference]
        private Transform LeftPopupPanelHolderTransform;

        [SerializeReference]
        private Transform CardHolderTransform;

        private List<PopupPanel> ActivePanels { get; set; } = new List<PopupPanel>();

        public static ReactionWindowContext? CurrentContext = null;

        [Obsolete("Transition to " + nameof(SetFromHoverListener))]
        public void SetFromHoverListener(IMouseHoverListener listener)
        {
            if (listener.TryGetCard(out Card cardToShow))
            {
                this.ShowCard(cardToShow);
            }
            else
            {
                this.HideCard();
            }

            if (listener.TryGetStatusEffect(out IStatusEffect effectToShow))
            {
                this.SetPopupPanels(effectToShow.DescribeStatusEffect(), listener.ShouldShowBase);
            }

            foreach (Graphic curGraphic in this.GetComponentsInChildren<Graphic>(true))
            {
                curGraphic.raycastTarget = false;
            }
        }

        public void _SetFromHoverListener(IMouseHoverListener listener)
        {
            if (listener._TryGetCard(out CardInstance cardToShow))
            {
                this._ShowCard(cardToShow);
            }
            else
            {
                this.HideCard();
            }

            if (listener._TryGetStatusEffect(out SpaceDeck.GameState.Minimum.AppliedStatusEffect effectToShow))
            {
                this._SetPopupPanels(effectToShow.GetDescription(), listener.ShouldShowBase);
            }

            foreach (Graphic curGraphic in this.GetComponentsInChildren<Graphic>(true))
            {
                curGraphic.raycastTarget = false;
            }
        }

        [Obsolete("Transition to " + nameof(_ShowCard))]
        private void ShowCard(Card toShow)
        {
            this.OverlayCard.gameObject.SetActive(true);
            this.OverlayCard.SetFromCard(toShow);
            this.CardHolderTransform.gameObject.SetActive(true);
            this.SetPopupPanelsFromCard(toShow);
        }

        private void _ShowCard(CardInstance toShow)
        {
            this.OverlayCard.gameObject.SetActive(true);
            this.OverlayCard._SetFromCard(toShow);
            this.CardHolderTransform.gameObject.SetActive(true);
            this._SetPopupPanelsFromCard(toShow);
        }

        private void HideCard()
        {
            this.OverlayCard.Annihilate();
            this.OverlayCard.gameObject.SetActive(false);
            this.CardHolderTransform.gameObject.SetActive(false);
        }

        [Obsolete("Transition to " + nameof(_SetPopupPanelsFromCard))]
        private void SetPopupPanelsFromCard(Card toShow)
        {
            this.SetPopupPanels(toShow.GetDescription(CurrentContext), false);
        }

        private void _SetPopupPanelsFromCard(CardInstance toShow)
        {
            this._SetPopupPanels(toShow.GetDescription(), false);
        }

        [Obsolete("Transition to " + nameof(_SetPopupPanelsFromCard))]
        private void SetPopupPanels(SFDDCards.EffectDescription description, bool includeBaseDescription = true)
        {
            for (int ii = this.RightPopupPanelHolderTransform.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.RightPopupPanelHolderTransform.GetChild(ii).gameObject);
            }

            for (int ii = this.LeftPopupPanelHolderTransform.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.LeftPopupPanelHolderTransform.GetChild(ii).gameObject);
            }

            for (int ii = this.ActivePanels.Count - 1; ii >= 0; ii--)
            {
                if (this.ActivePanels[ii] != null)
                { 
                    Destroy(this.ActivePanels[ii].gameObject);
                }
            }

            this.ActivePanels.Clear();
            Transform holder = this.RightPopupPanelHolderTransform;
            bool usingLeftHolder = false;

            if (this.transform.position.x >= Screen.width / 2f)
            {
                holder = this.LeftPopupPanelHolderTransform;
                usingLeftHolder = true;
            }

            List<SFDDCards.EffectDescription> descriptions = new List<SFDDCards.EffectDescription>();

            if (includeBaseDescription)
            {
                descriptions.Add(description);
            }

            foreach (SFDDCards.EffectDescription innerDescription in description.GetInnerDescriptions())
            {
                if (!innerDescription.Equals(description))
                {
                    descriptions.Add(innerDescription);
                }
            }

            foreach (SFDDCards.EffectDescription currentDescription in descriptions)
            {
                StringBuilder currentDescriptionText = new StringBuilder();

                if (!string.IsNullOrEmpty(currentDescription.DescribingLabel))
                {
                    currentDescriptionText.AppendLine($"<b>{currentDescription.DescribingLabel}</b>");
                }

                currentDescriptionText.Append(currentDescription.BreakDescriptionsIntoString());

                PopupPanel panel = Instantiate(this.PopupPanelPF, holder);
                panel.SetText(currentDescriptionText.ToString());
                this.ActivePanels.Add(panel);

                if (usingLeftHolder)
                {
                    panel.LayoutGroup.childAlignment = TextAnchor.UpperRight;
                }
            }
        }

        private void _SetPopupPanels(GameState.Minimum.EffectDescription description, bool includeBaseDescription = true)
        {
            for (int ii = this.RightPopupPanelHolderTransform.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.RightPopupPanelHolderTransform.GetChild(ii).gameObject);
            }

            for (int ii = this.LeftPopupPanelHolderTransform.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.LeftPopupPanelHolderTransform.GetChild(ii).gameObject);
            }

            for (int ii = this.ActivePanels.Count - 1; ii >= 0; ii--)
            {
                if (this.ActivePanels[ii] != null)
                {
                    Destroy(this.ActivePanels[ii].gameObject);
                }
            }

            this.ActivePanels.Clear();
            Transform holder = this.RightPopupPanelHolderTransform;
            bool usingLeftHolder = false;

            if (this.transform.position.x >= Screen.width / 2f)
            {
                holder = this.LeftPopupPanelHolderTransform;
                usingLeftHolder = true;
            }

            List<GameState.Minimum.EffectDescription> descriptions = new List<GameState.Minimum.EffectDescription>();

            if (includeBaseDescription)
            {
                descriptions.Add(description);
            }

            foreach (GameState.Minimum.EffectDescription innerDescription in description.MentionedDescribables)
            {
                if (!innerDescription.Equals(description))
                {
                    descriptions.Add(innerDescription);
                }
            }

            foreach (GameState.Minimum.EffectDescription currentDescription in descriptions)
            {
                StringBuilder currentDescriptionText = new StringBuilder();

                if (!string.IsNullOrEmpty(currentDescription.DescribingLabel))
                {
                    currentDescriptionText.AppendLine($"<b>{currentDescription.DescribingLabel}</b>");
                }

                currentDescriptionText.Append(currentDescription.BreakDescriptionsIntoString());

                PopupPanel panel = Instantiate(this.PopupPanelPF, holder);
                panel.SetText(currentDescriptionText.ToString());
                this.ActivePanels.Add(panel);

                if (usingLeftHolder)
                {
                    panel.LayoutGroup.childAlignment = TextAnchor.UpperRight;
                }
            }
        }

        public List<RectTransform> GetAllRectTransforms()
        {
            List<RectTransform> transforms = new List<RectTransform>();

            transforms.Add(this.OverlayCard.GetComponent<RectTransform>());

            foreach (PopupPanel panel in this.ActivePanels)
            {
                transforms.Add(panel.GetComponent<RectTransform>());
            }

            return transforms;
        }
    }
}