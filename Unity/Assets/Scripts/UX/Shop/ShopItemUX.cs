namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.UX;
    using SpaceDeck.UX.AssetLookup;

    public class ShopItemUX : MonoBehaviour
    {
        public IShopEntry RepresentingEntry { get; set; }

        [SerializeReference]
        private RewardCardUX RewardCardPF;

        [SerializeReference]
        private RewardArtifactUX RewardArtifactPF;

        [SerializeReference]
        private RewardCurrencyUX RewardCurrencyPF;

        [SerializeReference]
        private Transform RewardCardHolder;

        [SerializeReference]
        private TMPro.TMP_Text CostsLabel;

        private Action<ShopItemUX> OnClickDelegate;

        [SerializeReference]
        private GameObject CanNotAffordOverlay;

        public void SetFromEntry(IGameStateMutator mutator, IShopEntry toRepresent, Action<ShopItemUX> onClickDelegate)
        {
            int gainedAmount = toRepresent.GetGainedAmount(mutator);

            this.RepresentingEntry = toRepresent;
            this.OnClickDelegate = onClickDelegate;
            this.RepresentCosts(toRepresent.Costs, mutator);

            if (toRepresent.GainedCard != null)
            {
                RewardCardUX thisCard = Instantiate(this.RewardCardPF, this.RewardCardHolder);
                thisCard.SetFromCard(toRepresent.GainedCard, (DisplayedCardUX card) => { this.OnClick(); });
                thisCard.SetQuantity(gainedAmount);
            }
            else if (toRepresent.GainedArtifact != null)
            {
                RewardArtifactUX rewardArtifact = Instantiate(this.RewardArtifactPF, this.RewardCardHolder);
                rewardArtifact.SetFromArtifact(toRepresent.GainedArtifact, (RewardArtifactUX artifact) => { this.OnClick(); }, gainedAmount);
            }
            else if (toRepresent.GainedCurrency != null)
            {
                RewardCurrencyUX rewardCurrency = Instantiate(this.RewardCurrencyPF, this.RewardCardHolder);
                rewardCurrency.SetFromCurrency(toRepresent.GainedCurrency, (RewardCurrencyUX currency) => { this.OnClick(); }, gainedAmount);
            }
        }

        public void OnClick()
        {
            this.OnClickDelegate.Invoke(this);
        }

        void RepresentCosts(List<IShopCost> costs, IGameStateMutator mutator)
        {
            if (costs.Count == 0)
            {
                this.CostsLabel.text = "Free!";
                return;
            }

            string startingSeparator = "";
            StringBuilder compositeCurrencies = new StringBuilder();
            foreach (IShopCost cost in costs)
            {
                int costAmount = cost.GetCost(mutator);
                compositeCurrencies.Append($"{startingSeparator}{costAmount.ToString()}\u00A0{SpriteLookup.GetNameAndMaybeIcon(cost.CurrencyType)}");
                startingSeparator = ", ";
            }
            this.CostsLabel.text = compositeCurrencies.ToString();
        }

        private void OnEnable()
        {
            // TODO: UPDATE UX
        }

        private void OnDisable()
        {
            // TODO: UPDATE UX
        }

        public void UpdateAffordability(IGameStateMutator mutator)
        {
            if (!mutator.CanAfford(this.RepresentingEntry.Costs))
            {
                this.CanNotAffordOverlay.gameObject.SetActive(true);
                return;
            }

            this.CanNotAffordOverlay.gameObject.SetActive(false);
        }
    }
}