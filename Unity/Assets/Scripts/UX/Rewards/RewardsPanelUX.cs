namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.Evaluation.Actual;
    using SFDDCards.ScriptingTokens;
    using SFDDCards.UX;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class RewardsPanelUX : MonoBehaviour
    {
        [SerializeReference]
        private PickXRewardPanelUX PickRewardUXPF;
        [SerializeReference]
        private Transform RewardPanelsHolder;

        [SerializeReference]
        private CentralGameStateController CentralGameStateControllerInstance;

        [Obsolete("Transition to " + nameof(_Rewards))]
        public Reward Rewards;
        public SpaceDeck.GameState.Minimum.Reward _Rewards;

        private void Awake()
        {
            this.Annihilate();
        }

        [Obsolete("Transition to " + nameof(_SetReward))]
        public void SetReward(Reward toReward)
        {
            this.gameObject.SetActive(true);
            this.Annihilate();

            this.Rewards = toReward;

            foreach (PickSomeReward pickReward in toReward.PickRewards)
            {
                PickXRewardPanelUX pickRewardPanel = Instantiate(this.PickRewardUXPF, this.RewardPanelsHolder);
                pickRewardPanel.RepresentPick(CentralGameStateControllerInstance.CurrentCampaignContext, this, pickReward);
            }
        }

        public void _SetReward(SpaceDeck.GameState.Minimum.Reward toReward)
        {
            throw new System.NotImplementedException();
        }

        void Annihilate()
        {
            for (int ii = this.RewardPanelsHolder.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.RewardPanelsHolder.GetChild(ii).gameObject);
            }

            this.Rewards = null;
        }

        public void GainReward(PickSomeRewardSlot slotChosen)
        {
            this.CentralGameStateControllerInstance.CurrentCampaignContext.Gain(slotChosen);
        }

        public void ClosePanel(PickXRewardPanelUX toClose)
        {
            Destroy(toClose.gameObject);
        }

        public void CloseAll()
        {
            this.gameObject.SetActive(false);
        }
    }
}