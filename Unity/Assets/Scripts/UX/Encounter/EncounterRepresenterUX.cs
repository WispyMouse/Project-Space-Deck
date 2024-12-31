namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using UnityEngine;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.UX;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class EncounterRepresenterUX : MonoBehaviour
    {
        [SerializeReference]
        private CentralGameStateController CentralGameStateControllerInstance;
        [SerializeReference]
        private GameplayUXController GameplayUXControllerInstance;

        [SerializeReference]
        private GameObject UXParent;

        [SerializeReference]
        private TMPro.TMP_Text NameLabel;
        [SerializeReference]
        private TMPro.TMP_Text DescriptionLabel;
        [SerializeReference]
        private Transform ButtonHolder;
        [SerializeReference]
        private EncounterDialogueButtonUX DialogueButtonPF;

        private LowercaseString _currentEncounterIndex = "intro";
        private EncounterState _representingModel = null;

        public void _RepresentEncounter(EncounterState toRepresent, IGameStateMutator mutator)
        {
            this._representingModel = toRepresent;
            this.NameLabel.text = toRepresent.EncounterName;

            this.SetEncounterIndex(WellknownEncounters.Intro, mutator);
            this.UXParent.SetActive(true);
        }

        public void SetEncounterIndex(LowercaseString index, IGameStateMutator mutator)
        {
            this._currentEncounterIndex = index;

            this.DescriptionLabel.text = this._representingModel.BuildEncounterDialogue(index, mutator);

            this.DestroyButtonHolderButtons();

            IReadOnlyList<EncounterOption> options = this._representingModel.GetOptions(index, mutator);
            foreach (EncounterOption option in options)
            {
                EncounterDialogueButtonUX button = Instantiate(this.DialogueButtonPF, this.ButtonHolder);
                EncounterOption hungOption = option;

                string unprocessedDialogueOption = option.Dialogue;

                button.SetEncounterDialogue(option.Dialogue, () => this.ChooseOption(hungOption));
            }
        }

        private void DestroyButtonHolderButtons()
        {
            for (int ii = this.ButtonHolder.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.ButtonHolder.GetChild(ii).gameObject);
            }
        }

        public void ChooseOption(EncounterOption option)
        {
            if (option.PossibleOutcomes == null || option.PossibleOutcomes.Count == 0)
            {
                // If there are no possible outcomes, treat it as though it was a leave
                this.GameplayUXControllerInstance.EncounterDialogueComplete(this._representingModel);
                return;
            }

            EncounterOptionOutcome outcome = option.PossibleOutcomes[0];

            // Find the first requirement with matching criteria
            // It could be the first value, especially if it has no requirements
            for (int ii = 0; ii < option.PossibleOutcomes.Count; ii++)
            {
                // TODO: CHECK CONDITIONS
                // FOR NOW, RETURN FIRST THING
                outcome = option.PossibleOutcomes[ii];
                break;
            }

            outcome.Apply(this.CentralGameStateControllerInstance.GameplayState, out LowercaseString destination);
            if (string.IsNullOrEmpty(destination.ToString()))
            {
                this.GameplayUXControllerInstance.EncounterDialogueComplete(_representingModel);
            }
            else
            {
                this.UXParent.SetActive(true);
                this.SetEncounterIndex(destination, this.CentralGameStateControllerInstance.GameplayState);
            }
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}