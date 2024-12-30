namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.UX;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ChoiceNodeOptionUX : MonoBehaviour
    {
        [System.Obsolete("Transition to " + nameof(_Representing))]
        public ChoiceNodeOption Representing;
        public SpaceDeck.GameState.Minimum.ChoiceNodeOption _Representing;
        private ChoiceNodeSelectorUX SelectorUx;

        [SerializeReference]
        private TMPro.TMP_Text NameLabel;
        [SerializeReference]
        private TMPro.TMP_Text DescriptionLabel;

        [System.Obsolete("Transition to " + nameof(_RepresentOption))]
        public void RepresentOption(ChoiceNodeSelectorUX selector, ChoiceNodeOption toRepresent)
        {
            this.SelectorUx = selector;
            this.Representing = toRepresent;

            this.NameLabel.text = toRepresent.GetName();
            this.DescriptionLabel.text = toRepresent.GetDescription();
        }

        public void _RepresentOption(ChoiceNodeSelectorUX selector, SpaceDeck.GameState.Minimum.ChoiceNodeOption toRepresent)
        {
            this.SelectorUx = selector;
            this._Representing = toRepresent;

            this.NameLabel.text = toRepresent.GetName();
            this.DescriptionLabel.text = toRepresent.GetDescription();
        }

        public void ChooseThis()
        {
            this.SelectorUx.NodeIsChosen(this);
        }
    }
}