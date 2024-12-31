namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using static SpaceDeck.GameState.Minimum.PickReward;
    using static SpaceDeck.GameState.Minimum.Reward;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Imports;

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

            foreach (Reward slot in toRepresent.RewardOptions)
            {
                Reward pulledOutSlot = slot;

                int amountToAward = pulledOutSlot.GetAmount(mutator);

                switch (pulledOutSlot.IdentityKind)
                {
                    case RewardIdentityKind.Currency:
                        RewardCurrencyUX rewardCurrency = Instantiate(this.RewardCurrencyPF, this.RewardCardHolder);
                        rewardCurrency.SetFromCurrency(CurrencyDatabase.Get(pulledOutSlot.Id), (RewardCurrencyUX currency) => { this.RewardSlotChosen(pulledOutSlot); }, amountToAward);
                        break;
                    case RewardIdentityKind.Artifact:
                        RewardArtifactUX rewardArtifact = Instantiate(this.RewardArtifactPF, this.RewardCardHolder);
                        rewardArtifact.SetFromArtifact(StatusEffectDatabase.GetInstance(pulledOutSlot.Id), (RewardArtifactUX artifact) => { this.RewardSlotChosen(pulledOutSlot); }, amountToAward);
                        break;
                    case RewardIdentityKind.Card:
                        RewardCardUX thisCard = Instantiate(this.RewardCardPF, this.RewardCardHolder);
                        thisCard.SetFromCard(CardDatabase.GetInstance(pulledOutSlot.Id), (DisplayedCardUX card) => { this.RewardSlotChosen(pulledOutSlot); });
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

        public void RewardSlotChosen(Reward rewards)
        {
            this.RewardsPanel.GainReward(rewards);
            this.PicksRemaining--;

            if (this.PicksRemaining <= 0)
            {
                this.Annihilate();
                this.RewardsPanel.ClosePanel(this);
            }
        }
    }
}