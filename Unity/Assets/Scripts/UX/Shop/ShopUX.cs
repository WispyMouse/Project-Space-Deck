namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using SpaceDeck.UX;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;

    public class ShopUX : MonoBehaviour
    {
        [SerializeReference]
        private ShopItemUX ShopItemUXPF;
        [SerializeReference]
        private Transform ShopItemUXHolderTransform;

        [SerializeReference]
        private CentralGameStateController CentralGameStateControllerInstance;

        private List<ShopItemUX> ActiveShopItemUXs { get; set; } = new List<ShopItemUX>();

        private void Awake()
        {
            this.DestroyItems();
        }

        void DestroyItems()
        {
            for (int ii = ShopItemUXHolderTransform.childCount - 1; ii >= 0; ii--)
            {
                Destroy(ShopItemUXHolderTransform.GetChild(ii).gameObject);
            }

            this.ActiveShopItemUXs.Clear();
        }

        public void ShopItemSelected(ShopItemUX selectedItem)
        {
            if (!this.CentralGameStateControllerInstance.GameplayState.CanAfford(selectedItem.RepresentingEntry.Costs))
            {
                // TODO LOG
                return;
            }

            this.CentralGameStateControllerInstance.GameplayState.PurchaseShopItem(selectedItem.RepresentingEntry);
            this.ActiveShopItemUXs.Remove(selectedItem);
            Destroy(selectedItem.gameObject);

            UpdateAffordability();
        }

        public void SetShopItems(IReadOnlyList<LinkedShopEntry> shopEntries)
        {
            this.DestroyItems();

            foreach (LinkedShopEntry curEntry in shopEntries)
            {
                ShopItemUX shopEntry = Instantiate(this.ShopItemUXPF, this.ShopItemUXHolderTransform);
                shopEntry.SetFromEntry(this.CentralGameStateControllerInstance.GameplayState, curEntry, ShopItemSelected);
                this.ActiveShopItemUXs.Add(shopEntry);
            }

            UpdateAffordability();
        }

        private void UpdateAffordability()
        {
            foreach (ShopItemUX item in this.ActiveShopItemUXs)
            {
                item.UpdateAffordability(this.CentralGameStateControllerInstance.GameplayState);
            }
        }
    }
}