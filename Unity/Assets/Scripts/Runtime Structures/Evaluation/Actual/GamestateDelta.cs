namespace SFDDCards.Evaluation.Actual
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;
    using SFDDCards.Evaluation.Actual;

    public class GamestateDelta
    {
        public List<DeltaEntry> DeltaEntries { get; set; } = new List<DeltaEntry>();

        public void ApplyDelta(CampaignContext campaignContext)
        {
            this.ContinueApplyingDelta(campaignContext, 0);
        }

        public void AppendDelta(GamestateDelta delta)
        {
            this.DeltaEntries.AddRange(delta.DeltaEntries);
        }

        void ContinueApplyingDelta(CampaignContext campaignContext, int index)
        {
            if (index >= this.DeltaEntries.Count)
            {
                return;
            }

            DeltaEntry curEntry = DeltaEntries[index];

            // If there is an interactive choice to make, kick that over to the global choice handler
            // When the choice is resolved, continue applying the delta
            // Choices mark when they've been chosen, so this will continue from where it currently is next
            if (curEntry.ChoiceToMake != null && !curEntry.ChoiceToMake.ResultIsChosen)
            {
                // Attempt to resolve the choice without the player's input, depending on how the choice kind resolves itself
                if (!curEntry.ChoiceToMake.TryFinalizeWithoutPlayerInput(curEntry))
                {
                    GlobalUpdateUX.PendingPlayerChoice = true;
                    GlobalUpdateUX.PlayerMustMakeChoice.Invoke(curEntry, curEntry.ChoiceToMake, () => this.ContinueApplyingDelta(campaignContext, index));
                    return;
                }
            }
            GlobalUpdateUX.PendingPlayerChoice = false;

            if (curEntry.RealizedOperationScriptingToken != null)
            {
                curEntry.RealizedOperationScriptingToken.ApplyToDelta(curEntry, curEntry?.MadeFromBuilder?.BasedOnConcept?.CreatedFromContext, out List<DeltaEntry> stackedDeltas);
                if (stackedDeltas != null)
                {
                    DeltaEntries.InsertRange(index, stackedDeltas);
                }
                curEntry.RealizedOperationScriptingToken = null;

                this.ContinueApplyingDelta(campaignContext, index);
                return;
            }

            List<GameplaySequenceEvent> sequences = new List<GameplaySequenceEvent>();

            int evaluatedIntensity = 0;
            if (curEntry.ConceptualIntensity != null)
            {
                curEntry.ConceptualIntensity.TryEvaluateValue(campaignContext, curEntry.MadeFromBuilder, out evaluatedIntensity);
            }

            if (curEntry.IntensityKindType == TokenEvaluatorBuilder.IntensityKind.Damage && evaluatedIntensity > 0)
            {
                sequences.Add(new GameplaySequenceEvent(
                () =>
                    {
                        campaignContext.CheckAndApplyReactionWindow(new ReactionWindowContext(curEntry.FromCampaign, KnownReactionWindows.DamageIncoming, curEntry));
                    })
                );
            }

            sequences.Add(new GameplaySequenceEvent(
            () =>
                {
                    foreach (ElementResourceChange change in curEntry.ElementResourceChanges)
                    {
                        campaignContext.CurrentCombatContext.ApplyElementResourceChange(curEntry.MadeFromBuilder, change);
                    }
                })
            );

            if (curEntry.IntensityKindType == TokenEvaluatorBuilder.IntensityKind.NumberOfCards)
            {
                if (curEntry.NumberOfCardsRelationType == TokenEvaluatorBuilder.NumberOfCardsRelation.Draw)
                {
                    sequences.Add(new GameplaySequenceEvent(
                        () =>
                        {
                            campaignContext.CurrentCombatContext.PlayerCombatDeck.DealCards(evaluatedIntensity);
                        })
                    );
                }
            }

            sequences.Add(new GameplaySequenceEvent(
            () =>
                {
                    curEntry.Target?.ApplyDelta(campaignContext, campaignContext.CurrentCombatContext, curEntry);
                    campaignContext.CheckAllStateEffectsAndKnockouts();
                })
            );

            sequences.Add(new GameplaySequenceEvent(
            () =>
                {
                    foreach (Action<GamestateDelta> actionToExecute in curEntry.ActionsToExecute)
                    {
                        actionToExecute.Invoke(this);
                    }
                })
            );

            sequences.Add(new GameplaySequenceEvent(
            () =>
            {
                this.ContinueApplyingDelta(campaignContext, index + 1);
            }));

            GlobalSequenceEventHolder.PushSequencesToTop(campaignContext, sequences.ToArray());
        }

        /// <summary>
        /// Determines the encounter index that would result from executing this Delta.
        /// If there are no destinations determined, this will return an empty string.
        /// The last destination to be in the list of deltas is the one that will be applied.
        /// This generally means that if a script had three different destinations, only
        /// the last one to be set will be used.
        /// </summary>
        /// <returns></returns>
        public string GetEncounterDestination()
        {
            string destination = string.Empty;

            foreach (DeltaEntry deltaEntry in this.DeltaEntries)
            {
                if (!string.IsNullOrWhiteSpace(deltaEntry.EncounterDialogueDestination))
                {
                    destination = deltaEntry.EncounterDialogueDestination;
                }
            }

            return destination;
        }
    }
}