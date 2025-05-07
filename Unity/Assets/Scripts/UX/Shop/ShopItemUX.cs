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
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;
    using System.Linq;

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

        public void SetFromEntry(IGameStateMutator mutator, LinkedShopEntry toRepresent, Action<ShopItemUX> onClickDelegate)
        {
            int gainedAmount = toRepresent.GetGainedAmount(mutator);

            this.RepresentingEntry = toRepresent;
            this.OnClickDelegate = onClickDelegate;
            this.RepresentCosts(toRepresent.Costs, mutator);

            switch (toRepresent.GainedReward.IdentityKind)
            {
                case RewardPrototype.RewardIdentityKind.Card:
                    if (toRepresent.LinkedGainedReward.GainedCards.Count != 1)
                    {
                        Logging.DebugLog(WellknownLoggingLevels.Warning, WellknownLoggingCategories.ShopItemUX, $"Asked to represent a number of cards other than 1, but the system isn't set up for it yet. Arbitrarily using first.");
                    }

                    RewardCardUX thisCard = Instantiate(this.RewardCardPF, this.RewardCardHolder);
                    thisCard.SetFromCard(toRepresent.LinkedGainedReward.GainedCards[0], (DisplayedCardUX card) => { this.OnClick(); });
                    thisCard.SetQuantity(toRepresent.LinkedGainedReward.GainedCards.Count());
                    break;
                case RewardPrototype.RewardIdentityKind.Artifact:
                    // RewardArtifactUX rewardArtifact = Instantiate(this.RewardArtifactPF, this.RewardCardHolder);
                    // rewardArtifact.SetFromArtifact(toRepresent.GainedArtifact, (RewardArtifactUX artifact) => { this.OnClick(); }, gainedAmount);
                    throw new System.NotImplementedException($"Oops, artifacts aren't implemented again yet!");
                    break;
                case RewardPrototype.RewardIdentityKind.Currency:
                    if (toRepresent.LinkedGainedReward.GainedCurrency.Count != 1)
                    {
                        Logging.DebugLog(WellknownLoggingLevels.Warning, WellknownLoggingCategories.ShopItemUX, $"Asked to represent a number of currencies other than 1, but the system isn't set up for it yet. Arbitrarily using something.");
                    }

                    List<KeyValuePair<Currency, int>> arbitaryOrderedList = toRepresent.LinkedGainedReward.GainedCurrency.ToList();
                    KeyValuePair<Currency, int> arbitrarilySelectedCurrency = arbitaryOrderedList[0];

                    RewardCurrencyUX rewardCurrency = Instantiate(this.RewardCurrencyPF, this.RewardCardHolder);
                    rewardCurrency.SetFromCurrency(arbitrarilySelectedCurrency.Key, (RewardCurrencyUX currency) => { this.OnClick(); }, gainedAmount);
                    break;
            }
        }

        public void OnClick()
        {
            this.OnClickDelegate.Invoke(this);
        }

        void RepresentCosts(IReadOnlyList<IShopCost> costs, IGameStateMutator mutator)
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