namespace SpaceDeck.UX
{
    using SpaceDeck.GameState.Minimum;
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

        public List<PickReward> Rewards;

        private void Awake()
        {
            this.Annihilate();
        }

        public void SetReward(List<PickReward> rewards)
        {
            this.gameObject.SetActive(true);
            this.Annihilate();

            this.Rewards = rewards;

            foreach (PickReward pickReward in this.Rewards)
            {
                PickXRewardPanelUX pickRewardPanel = Instantiate(this.PickRewardUXPF, this.RewardPanelsHolder);
                pickRewardPanel.RepresentPick(CentralGameStateControllerInstance.GameplayState, this, pickReward);
            }
        }

        void Annihilate()
        {
            for (int ii = this.RewardPanelsHolder.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.RewardPanelsHolder.GetChild(ii).gameObject);
            }

            this.Rewards = null;
        }

        public void GainReward(Reward reward)
        {
            this.CentralGameStateControllerInstance.GameplayState.Gain(reward);
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