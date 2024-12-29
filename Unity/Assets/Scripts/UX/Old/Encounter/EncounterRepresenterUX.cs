namespace SFDDCards.UX
{
    using SFDDCards.Evaluation.Actual;
    using SFDDCards.ImportModels;
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

        [Obsolete("Should transition to " + nameof(_currentEncounterIndex))]
        private string currentEncounterIndex = "intro";
        private LowercaseString _currentEncounterIndex = "intro";

        [Obsolete("Should transition to " + nameof(_representingModel))]
        private EvaluatedEncounter representingModel = null;
        private EncounterState _representingModel = null;

        [Obsolete("Should transition to " + nameof(_RepresentEncounter))]
        public void RepresentEncounter(EvaluatedEncounter toRepresent)
        {
            this.representingModel = toRepresent;
            this.NameLabel.text = toRepresent.BasedOn.Name;

            this.SetEncounterIndex("intro");
            this.UXParent.SetActive(true);
        }

        public void _RepresentEncounter(EncounterState toRepresent, IGameStateMutator mutator)
        {
            this._representingModel = toRepresent;
            this.NameLabel.text = toRepresent.EncounterName;

            this._SetEncounterIndex("intro", mutator);
            this.UXParent.SetActive(true);
        }

        [Obsolete("Should transition to " + nameof(SetEncounterIndex))]
        public void SetEncounterIndex(string index)
        {
            this.currentEncounterIndex = index;

            string unprocessedDialogue = this.representingModel.BasedOn.BuildEncounterDialogue(index, this.CentralGameStateControllerInstance.CurrentCampaignContext);
            this.DescriptionLabel.text = EffectDescriberDatabase.ReplaceTokensInString(unprocessedDialogue, this.CentralGameStateControllerInstance.CurrentCampaignContext);

            this.DestroyButtonHolderButtons();

            List<EncounterOptionImport> options = this.representingModel.BasedOn.GetOptions(index, this.CentralGameStateControllerInstance.CurrentCampaignContext);
            foreach (EncounterOptionImport option in options)
            {
                EncounterDialogueButtonUX button = Instantiate(this.DialogueButtonPF, this.ButtonHolder);
                EncounterOptionImport hungOption = option;

                string unprocessedDialogueOption = option.Dialogue;

                button.SetEncounterDialogue(EffectDescriberDatabase.ReplaceTokensInString(unprocessedDialogueOption, this.CentralGameStateControllerInstance.CurrentCampaignContext), () => this.ChooseOption(hungOption));
            }
        }

        public void _SetEncounterIndex(LowercaseString index, IGameStateMutator mutator)
        {
            this._currentEncounterIndex = index;

            string unprocessedDialogue = this._representingModel.BuildEncounterDialogue(index, mutator);
            this.DescriptionLabel.text = EffectDescriberDatabase.ReplaceTokensInString(unprocessedDialogue, this.CentralGameStateControllerInstance.CurrentCampaignContext);

            this.DestroyButtonHolderButtons();

            IReadOnlyList<EncounterOption> options = this._representingModel.GetOptions(index, mutator);
            foreach (EncounterOption option in options)
            {
                EncounterDialogueButtonUX button = Instantiate(this.DialogueButtonPF, this.ButtonHolder);
                EncounterOption hungOption = option;

                string unprocessedDialogueOption = option.Dialogue;

                button.SetEncounterDialogue(EffectDescriberDatabase.ReplaceTokensInString(unprocessedDialogueOption, this.CentralGameStateControllerInstance.CurrentCampaignContext), () => this.ChooseOption(hungOption));
            }
        }

        private void DestroyButtonHolderButtons()
        {
            for (int ii = this.ButtonHolder.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.ButtonHolder.GetChild(ii).gameObject);
            }
        }

        [Obsolete("Should transition to the version of " + nameof(ChooseOption) + " that uses EncounterOption")]
        public void ChooseOption(EncounterOptionImport option)
        {
            if (option.PossibleOutcomes == null || option.PossibleOutcomes.Count == 0)
            {
                // If there are no possible outcomes, treat it as though it was a leave
                GlobalSequenceEventHolder.PushSequenceToTop(new GameplaySequenceEvent(
                () =>
                {
                    this.GameplayUXControllerInstance.EncounterDialogueComplete(this.representingModel);
                }));
                return;
            }

            EncounterOptionOutcomeImport outcome = option.PossibleOutcomes[0];

            // Find the first requirement with matching criteria
            // It could be the first value, especially if it has no requirements
            for (int ii = 0; ii < option.PossibleOutcomes.Count; ii++)
            {
                if (this.CentralGameStateControllerInstance.CurrentCampaignContext.RequirementsAreMet(option.PossibleOutcomes[ii].Criteria))
                {
                    outcome = option.PossibleOutcomes[ii];
                    break;
                }
            }

            GamestateDelta delta = ScriptTokenEvaluator.GetDeltaFromTokens(outcome.Effect,
                this.CentralGameStateControllerInstance.CurrentCampaignContext,
                null,
                this.CentralGameStateControllerInstance.CurrentCampaignContext.CampaignPlayer,
                this.CentralGameStateControllerInstance.CurrentCampaignContext.CampaignPlayer);

            string destination = delta.GetEncounterDestination();

            // Push resolving this event, and then afterwards, continuing the dialogue
            this.UXParent.SetActive(false);
            GlobalSequenceEventHolder.PushSequencesToTop(
                this.CentralGameStateControllerInstance.CurrentCampaignContext,
                new GameplaySequenceEvent(
                () =>
                {
                    GlobalUpdateUX.LogTextEvent.Invoke(EffectDescriberDatabase.DescribeResolvedEffect(delta), GlobalUpdateUX.LogType.GameEvent);
                    delta.ApplyDelta(this.CentralGameStateControllerInstance.CurrentCampaignContext);
                }),
                new GameplaySequenceEvent(
                () =>
                {
                    if (string.IsNullOrEmpty(destination))
                    {
                        this.GameplayUXControllerInstance.EncounterDialogueComplete(representingModel);
                    }
                    else
                    {
                        this.UXParent.SetActive(true);
                        this.SetEncounterIndex(destination);
                    }
                }));
        }

        public void ChooseOption(EncounterOption option)
        {
            if (option.PossibleOutcomes == null || option.PossibleOutcomes.Count == 0)
            {
                // If there are no possible outcomes, treat it as though it was a leave
                GlobalSequenceEventHolder.PushSequenceToTop(new GameplaySequenceEvent(
                () =>
                {
                    this.GameplayUXControllerInstance.EncounterDialogueComplete(this.representingModel);
                }));
                return;
            }

            EncounterOptionOutcome outcome = option.PossibleOutcomes[0];

            // Find the first requirement with matching criteria
            // It could be the first value, especially if it has no requirements
            for (int ii = 0; ii < option.PossibleOutcomes.Count; ii++)
            {
                if (this.CentralGameStateControllerInstance.CurrentCampaignContext.RequirementsAreMet(option.PossibleOutcomes[ii].RequirementScript.ToString()))
                {
                    outcome = option.PossibleOutcomes[ii];
                    break;
                }
            }

            GamestateDelta delta = ScriptTokenEvaluator.GetDeltaFromTokens(outcome.RequirementScript.ToString(),
                this.CentralGameStateControllerInstance.CurrentCampaignContext,
                null,
                this.CentralGameStateControllerInstance.CurrentCampaignContext.CampaignPlayer,
                this.CentralGameStateControllerInstance.CurrentCampaignContext.CampaignPlayer);

            string destination = delta.GetEncounterDestination();

            // Push resolving this event, and then afterwards, continuing the dialogue
            this.UXParent.SetActive(false);
            GlobalSequenceEventHolder.PushSequencesToTop(
                this.CentralGameStateControllerInstance.CurrentCampaignContext,
                new GameplaySequenceEvent(
                () =>
                {
                    GlobalUpdateUX.LogTextEvent.Invoke(EffectDescriberDatabase.DescribeResolvedEffect(delta), GlobalUpdateUX.LogType.GameEvent);
                    delta.ApplyDelta(this.CentralGameStateControllerInstance.CurrentCampaignContext);
                }),
                new GameplaySequenceEvent(
                () =>
                {
                    if (string.IsNullOrEmpty(destination))
                    {
                        this.GameplayUXControllerInstance.EncounterDialogueComplete(representingModel);
                    }
                    else
                    {
                        this.UXParent.SetActive(true);
                        this.SetEncounterIndex(destination);
                    }
                }));
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}