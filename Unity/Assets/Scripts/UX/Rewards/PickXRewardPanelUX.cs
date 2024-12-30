namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.UX;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Imports;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using static SpaceDeck.GameState.Minimum.PickReward;
    using static SpaceDeck.GameState.Minimum.Reward;

    public class PickXRewardPanelUX : MonoBehaviour
    {
        [Obsolete("Transition to " + nameof(_BasedOnPick))]
        public PickSomeReward BasedOnPick { get; set; }
        public PickReward _BasedOnPick { get; set; }
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

        [Obsolete("Transition to " + nameof(_RepresentPick))]
        public void RepresentPick(CampaignContext campaignContext, RewardsPanelUX rewardsPanel, PickSomeReward toRepresent)
        {
            this.BasedOnPick = toRepresent;
            this.RewardsPanel = rewardsPanel;
            this.PicksRemaining = toRepresent.BasedOn.ProtocolArgument;

            this.Annihilate();

            switch (toRepresent.Protocol)
            {
                case PickRewardProtocol.ChooseX:
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
                    rewardArtifact.SetFromArtifact(slot._GainedEffect, (RewardArtifactUX artifact) => { this.RewardSlotChosen(pulledOutSlot); }, amountToAward);
                }
                else if (slot.GainedCurrency != null)
                {
                    RewardCurrencyUX rewardCurrency = Instantiate(this.RewardCurrencyPF, this.RewardCardHolder);
                    rewardCurrency._SetFromCurrency(slot.GainedCurrency, (RewardCurrencyUX currency) => { this.RewardSlotChosen(pulledOutSlot); }, amountToAward);
                }
            }
        }

        public void _RepresentPick(IGameStateMutator mutator, RewardsPanelUX rewardsPanel, SpaceDeck.GameState.Minimum.PickReward toRepresent)
        {
            this._BasedOnPick = toRepresent;
            this.RewardsPanel = rewardsPanel;
            this.PicksRemaining = toRepresent.ProtocolArgument;

            this.Annihilate();

            switch (toRepresent.Protocol)
            {
                case PickRewardProtocol.ChooseX:
                    this.ExplanationLabel.text = $"Choose up to {toRepresent.ProtocolArgument}";
                    break;
            }

            foreach (SpaceDeck.GameState.Minimum.Reward slot in toRepresent.RewardOptions)
            {
                SpaceDeck.GameState.Minimum.Reward pulledOutSlot = slot;

                int amountToAward = pulledOutSlot.GetAmount(mutator);

                switch (pulledOutSlot.IdentityKind)
                {
                    case RewardIdentityKind.Currency:
                        RewardCurrencyUX rewardCurrency = Instantiate(this.RewardCurrencyPF, this.RewardCardHolder);
                        rewardCurrency._SetFromCurrency(CurrencyDatabase.Get(pulledOutSlot.Id), (RewardCurrencyUX currency) => { this._RewardSlotChosen(pulledOutSlot); }, amountToAward);
                        break;
                    case RewardIdentityKind.Artifact:
                        RewardArtifactUX rewardArtifact = Instantiate(this.RewardArtifactPF, this.RewardCardHolder);
                        rewardArtifact.SetFromArtifact(SpaceDeck.Models.Databases.StatusEffectDatabase.GetInstance(pulledOutSlot.Id), (RewardArtifactUX artifact) => { this._RewardSlotChosen(pulledOutSlot); }, amountToAward);
                        break;
                    case RewardIdentityKind.Card:
                        RewardCardUX thisCard = Instantiate(this.RewardCardPF, this.RewardCardHolder);
                        thisCard._SetFromCard(SpaceDeck.Models.Databases.CardDatabase.GetInstance(pulledOutSlot.Id), (DisplayedCardUX card) => { this._RewardSlotChosen(pulledOutSlot); });
                        thisCard.SetQuantity(amountToAward);
                        break;
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

        [Obsolete("Transition to " + nameof(_RewardSlotChosen))]
        public void RewardSlotChosen(PickSomeRewardSlot slotChosen)
        {
            // TODO: DISMANTLE
            // this.RewardsPanel.GainReward(slotChosen);
            this.PicksRemaining--;

            if (this.PicksRemaining <= 0)
            {
                this.Annihilate();
                this.RewardsPanel.ClosePanel(this);
            }
        }

        public void _RewardSlotChosen(SpaceDeck.GameState.Minimum.Reward rewards)
        {
            this.RewardsPanel._GainReward(rewards);
            this.PicksRemaining--;

            if (this.PicksRemaining <= 0)
            {
                this.Annihilate();
                this.RewardsPanel.ClosePanel(this);
            }
        }
    }
}