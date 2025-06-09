namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using static SpaceDeck.GameState.Minimum.PickReward;
    using static SpaceDeck.GameState.Minimum.RewardPrototype;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Instances;

    public class PickXRewardPanelUX : MonoBehaviour
    {
        public PickReward BasedOnPick { get; set; }
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

        public void RepresentPick(IGameStateMutator mutator, RewardsPanelUX rewardsPanel, PickReward toRepresent)
        {
            this.BasedOnPick = toRepresent;
            this.RewardsPanel = rewardsPanel;
            this.PicksRemaining = toRepresent.ProtocolArgument;

            this.Annihilate();

            switch (toRepresent.Protocol)
            {
                case PickRewardProtocol.ChooseX:
                    this.ExplanationLabel.text = $"Choose up to {toRepresent.ProtocolArgument}";
                    break;
            }

            foreach (RewardPrototype slot in toRepresent.RewardOptions)
            {
                RewardPrototype pulledOutSlot = slot;
                LinkedRewardInstance rewardInstance = RewardDatabase.GetReward(pulledOutSlot);

                int amountToAward = pulledOutSlot.GetAmount(mutator);

                switch (pulledOutSlot.IdentityKind)
                {
                    case RewardIdentityKind.Currency:
                        RewardCurrencyUX rewardCurrency = Instantiate(this.RewardCurrencyPF, this.RewardCardHolder);
                        rewardCurrency.SetFromCurrency(rewardInstance, CurrencyDatabase.Get(pulledOutSlot.Id), (RewardCurrencyUX currency) => { this.RewardSlotChosen(rewardCurrency); }, amountToAward);
                        break;
                    case RewardIdentityKind.Artifact:
                        RewardArtifactUX rewardArtifact = Instantiate(this.RewardArtifactPF, this.RewardCardHolder);
                        rewardArtifact.SetFromArtifact(rewardInstance, StatusEffectDatabase.GetInstance(pulledOutSlot.Id), (RewardArtifactUX artifact) => { this.RewardSlotChosen(rewardArtifact); }, amountToAward);
                        break;
                    case RewardIdentityKind.Card:
                        RewardCardUX thisCard = Instantiate(this.RewardCardPF, this.RewardCardHolder);
                        thisCard.SetFromCard(rewardInstance, CardDatabase.GetInstance(pulledOutSlot.Id), (DisplayedCardUX card) => { this.RewardSlotChosen(thisCard); });
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

        public void RewardSlotChosen(RewardCurrencyUX currencyReward)
        {
            this.GainReward(currencyReward.RewardInstance);
            Destroy(currencyReward.gameObject);
        }

        public void RewardSlotChosen(RewardArtifactUX artifactReward)
        {
            this.GainReward(artifactReward.RewardInstance);
            Destroy(artifactReward.gameObject);
        }

        public void RewardSlotChosen(RewardCardUX cardReward)
        {
            this.GainReward(cardReward.RewardInstance);
            Destroy(cardReward.gameObject);
        }

        private void GainReward(LinkedRewardInstance rewardInstance)
        {
            this.RewardsPanel.GainReward(rewardInstance);
            this.PicksRemaining--;

            if (this.PicksRemaining <= 0)
            {
                this.Annihilate();
                this.RewardsPanel.ClosePanel(this);
            }
        }
    }
}