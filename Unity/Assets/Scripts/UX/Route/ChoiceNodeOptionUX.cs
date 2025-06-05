namespace SpaceDeck.UX
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ChoiceNodeOptionUX : MonoBehaviour
    {
        public LinkedChoiceNodeOption Representing;
        public EncounterInstance RepresentingEncounter => this.Representing.WillEncounter;
        private ChoiceNodeSelectorUX SelectorUx;

        [SerializeReference]
        private TMPro.TMP_Text NameLabel;
        [SerializeReference]
        private TMPro.TMP_Text DescriptionLabel;

        public void RepresentOption(ChoiceNodeSelectorUX selector, LinkedChoiceNodeOption toRepresent)
        {
            this.SelectorUx = selector;
            this.Representing = toRepresent;

            this.NameLabel.text = this.RepresentingEncounter.EncounterName;
            this.DescriptionLabel.text = this.RepresentingEncounter.EncounterDescription;
        }

        public void ChooseThis()
        {
            this.SelectorUx.NodeIsChosen(this);
        }
    }
}