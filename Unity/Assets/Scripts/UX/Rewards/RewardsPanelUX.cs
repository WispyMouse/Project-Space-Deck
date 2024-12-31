namespace SpaceDeck.UX
{
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

        public List<SpaceDeck.GameState.Minimum.PickReward> _Rewards;

        private void Awake()
        {
            this.Annihilate();
        }

        public void _SetReward(List<SpaceDeck.GameState.Minimum.PickReward> rewards)
        {
            this.gameObject.SetActive(true);
            this.Annihilate();

            this._Rewards = rewards;

            foreach (SpaceDeck.GameState.Minimum.PickReward pickReward in this._Rewards)
            {
                PickXRewardPanelUX pickRewardPanel = Instantiate(this.PickRewardUXPF, this.RewardPanelsHolder);
                pickRewardPanel._RepresentPick(CentralGameStateControllerInstance.GameplayState, this, pickReward);
            }
        }

        void Annihilate()
        {
            for (int ii = this.RewardPanelsHolder.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.RewardPanelsHolder.GetChild(ii).gameObject);
            }

            this._Rewards = null;
        }

        public void _GainReward(SpaceDeck.GameState.Minimum.Reward reward)
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