namespace SpaceDeck.UX
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SpaceDeck.UX;

    public class ChoiceNodeSelectorUX : MonoBehaviour
    {
        public SpaceDeck.GameState.Minimum.ChoiceNode _CurrentlyRepresenting;

        [SerializeReference]
        private ChoiceNodeOptionUX OptionPrefab;

        [SerializeReference]
        private Transform TransformParent;

        private List<ChoiceNodeOptionUX> ActiveOptions { get; set; } = new List<ChoiceNodeOptionUX>();

        [SerializeReference]
        private GameplayUXController UXController;

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

        public void _NodeIsChosen(ChoiceNodeOptionUX ux)
        {
            UXController._NodeIsChosen(ux._Representing);
            this.ClearNodes();
        }
    }
}