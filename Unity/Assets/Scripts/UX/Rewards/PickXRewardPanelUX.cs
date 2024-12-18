namespace SFDDCards.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class PickXRewardPanelUX : MonoBehaviour
    {
        public PickSomeReward BasedOnPick { get; set; }
        public RewardsPanelUX RewardsPanel { get; set; }

        [SerializeReference]
        private TMPro.TMP_Text ExplanationLabel;

        [SerializeReference]
        private RewardCardUX RewardCardPF;

        [SerializeReference]
        private RewardArtifactUX RewardArtifactPF;

        [SerializeReference]
        private RewardCurrencyUX RewardCurrencyPF;

        [SerializeReference]
        private Transform RewardCardHolder;

        int PicksRemaining { get; set; } = 0;

        public void RepresentPick(CampaignContext campaignContext, RewardsPanelUX rewardsPanel, PickSomeReward toRepresent)
        {
            this.BasedOnPick = toRepresent;
            this.RewardsPanel = rewardsPanel;
            this.PicksRemaining = toRepresent.BasedOn.ProtocolArgument;

            this.Annihilate();

            switch (toRepresent.Protocol)
            {
                case PickRewardImport.PickRewardProtocol.ChooseX:
                    this.ExplanationLabel.text = $"Choose up to {this.BasedOnPick.BasedOn.ProtocolArgument}";
                    break;
            }

            foreach (PickSomeRewardSlot slot in this.BasedOnPick.PickRewardSlots)
            {
                PickSomeRewardSlot pulledOutSlot = slot;

                int amountToAward;
                if (!slot.GainedAmount.TryEvaluateValue(campaignContext, null, out amountToAward))
                {
                    amountToAward = 1;
                }
                slot.AmountToAwardEvaluated = amountToAward;

                if (slot.GainedCard != null)
                {
                    RewardCardUX thisCard = Instantiate(this.RewardCardPF, this.RewardCardHolder);
                    thisCard.SetFromCard(slot.GainedCard, (DisplayedCardUX card) => { this.RewardSlotChosen(pulledOutSlot); });
                    thisCard.SetQuantity(amountToAward);
                }
                else if (slot.GainedEffect != null)
                {
                    RewardArtifactUX rewardArtifact = Instantiate(this.RewardArtifactPF, this.RewardCardHolder);
                    rewardArtifact.SetFromArtifact(slot.GainedEffect, (RewardArtifactUX artifact) => { this.RewardSlotChosen(pulledOutSlot); }, amountToAward);
                }
                else if (slot.GainedCurrency != null)
                {
                    RewardCurrencyUX rewardCurrency = Instantiate(this.RewardCurrencyPF, this.RewardCardHolder);
                    rewardCurrency.SetFromCurrency(slot.GainedCurrency, (RewardCurrencyUX currency) => { this.RewardSlotChosen(pulledOutSlot); }, amountToAward);
                }
            }
        }

        private void Awake()
        {
            this.Annihilate();
        }

        void Annihilate()
        {
            for (int ii = this.RewardCardHolder.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.RewardCardHolder.GetChild(ii).gameObject);
            }
        }

        public void RewardSlotChosen(PickSomeRewardSlot slotChosen)
        {
            this.RewardsPanel.GainReward(slotChosen);
            this.PicksRemaining--;

            if (this.PicksRemaining <= 0)
            {
                this.Annihilate();
                this.RewardsPanel.ClosePanel(this);
            }
        }
    }
}