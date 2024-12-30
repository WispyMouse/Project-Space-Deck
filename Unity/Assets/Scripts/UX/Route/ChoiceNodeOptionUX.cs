namespace SpaceDeck.UX
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ChoiceNodeOptionUX : MonoBehaviour
    {
        public SpaceDeck.GameState.Minimum.ChoiceNodeOption _Representing;
        private ChoiceNodeSelectorUX SelectorUx;

        [SerializeReference]
        private TMPro.TMP_Text NameLabel;
        [SerializeReference]
        private TMPro.TMP_Text DescriptionLabel;

        public void _RepresentOption(ChoiceNodeSelectorUX selector, SpaceDeck.GameState.Minimum.ChoiceNodeOption toRepresent)
        {
            this.SelectorUx = selector;
            this._Representing = toRepresent;

            this.NameLabel.text = toRepresent.GetName();
            this.DescriptionLabel.text = toRepresent.GetDescription();
        }

        public void _ChooseThis()
        {
            this.SelectorUx._NodeIsChosen(this);
        }
    }
}