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

        public SpaceDeck.GameState.Minimum.Reward _Rewards;

        private void Awake()
        {
            this.Annihilate();
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

            this._Rewards = null;
        }

        public void _GainReward(SpaceDeck.GameState.Minimum.Reward reward)
        {
            this.CentralGameStateControllerInstance.CurrentCampaignContext.Gain(reward);
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