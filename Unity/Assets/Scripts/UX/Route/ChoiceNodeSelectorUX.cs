namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.UX;
    using SpaceDeck.UX;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ChoiceNodeSelectorUX : MonoBehaviour
    {
        [System.Obsolete("Transition to " + nameof(_CurrentlyRepresenting))]
        public ChoiceNode CurrentlyRepresenting;
        public SpaceDeck.GameState.Minimum.ChoiceNode _CurrentlyRepresenting;

        [SerializeReference]
        private ChoiceNodeOptionUX OptionPrefab;

        [SerializeReference]
        private Transform TransformParent;

        private List<ChoiceNodeOptionUX> ActiveOptions { get; set; } = new List<ChoiceNodeOptionUX>();

        [SerializeReference]
        private GameplayUXController UXController;

        [System.Obsolete("Transition to " + nameof(_RepresentNode))]
        public void RepresentNode(ChoiceNode toRepresent)
        {
            this.ClearNodes();
            this.CurrentlyRepresenting = toRepresent;

            foreach (ChoiceNodeOption option in toRepresent.Options)
            {
                ChoiceNodeOptionUX optionUx = Instantiate(this.OptionPrefab, this.TransformParent);
                optionUx.RepresentOption(this, option);
                this.ActiveOptions.Add(optionUx);
            }
        }

        public void _RepresentNode(SpaceDeck.GameState.Minimum.ChoiceNode toRepresent)
        {
            this.ClearNodes();
            this._CurrentlyRepresenting = toRepresent;

            foreach (SpaceDeck.GameState.Minimum.ChoiceNodeOption option in toRepresent.Options)
            {
                ChoiceNodeOptionUX optionUx = Instantiate(this.OptionPrefab, this.TransformParent);
                optionUx._RepresentOption(this, option);
                this.ActiveOptions.Add(optionUx);
            }
        }

        public void ClearNodes()
        {
            for (int ii = this.ActiveOptions.Count - 1; ii >= 0; ii--)
            {
                Destroy(this.ActiveOptions[ii].gameObject);
            }
            this.ActiveOptions.Clear();
        }

        [System.Obsolete("Transition to " + nameof(_NodeIsChosen))]
        public void NodeIsChosen(ChoiceNodeOptionUX ux)
        {
            UXController.NodeIsChosen(ux.Representing);
            this.ClearNodes();
        }

        public void _NodeIsChosen(ChoiceNodeOptionUX ux)
        {
            UXController._NodeIsChosen(ux._Representing);
            this.ClearNodes();
        }
    }
}