namespace SpaceDeck.GameState.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    /// <summary>
    /// Describes the state of a encounter.
    /// 
    /// This is meant to describe every aspect of an encounter, from
    /// cards in hand to enemies involved and their statistics. This
    /// is scoped to a single Encounter, exposing some configuration
    /// not possible in <see cref="GameState"/>. A Gamestate optionally
    /// holds an EncounterState.
    /// 
    /// This class is meant to be cloneable, allowing for theoretical
    /// branching of battle states.
    /// </summary>
    public class EncounterState
    {
        public string EncounterName;
        public readonly List<Entity> EncounterEntities = new List<Entity>();
        public Dictionary<CardInstance, LowercaseString> CardsInZones = new Dictionary<CardInstance, LowercaseString>();
        public Dictionary<LowercaseString, List<CardInstance>> ZonesWithCards = new Dictionary<LowercaseString, List<CardInstance>>();

        public virtual bool HasEncounterDialogue => false;
        public virtual bool IsShopEncounter => false;

        public void MoveCard(CardInstance card, LowercaseString zone)
        {
            if (!this.ZonesWithCards.ContainsKey(zone))
            {
                this.ZonesWithCards.Add(zone, new List<CardInstance>());
            }

            if (this.CardsInZones.ContainsKey(card))
            {
                // This is already tracked, so we should remove it from its previous zone
                LowercaseString previousZone = this.CardsInZones[card];
                this.ZonesWithCards[previousZone].Remove(card);
                this.CardsInZones[card] = zone;
                this.ZonesWithCards[zone].Add(card);
            }
            else
            {
                // This isn't tracked, so we need to consider adding it to lists
                this.CardsInZones.Add(card, zone);
                this.ZonesWithCards[zone].Add(card);
            }
        }

        public LowercaseString GetCardZone(CardInstance card)
        {
            if (this.CardsInZones.TryGetValue(card, out LowercaseString zone))
            {
                return zone;
            }

            return string.Empty;
        }

        public IReadOnlyList<CardInstance> GetZoneCards(LowercaseString zone)
        {
            if (this.ZonesWithCards.TryGetValue(zone, out List<CardInstance> cards))
            {
                return cards;
            }
            this.ZonesWithCards.Add(zone, new List<CardInstance>());
            return this.GetZoneCards(zone);
        }

        public virtual string BuildEncounterDialogue(LowercaseString index, IGameStateMutator mutator)
        {
            return string.Empty;
        }

        public virtual IReadOnlyList<EncounterOption> GetOptions(LowercaseString index, IGameStateMutator mutator)
        {
            return Array.Empty<EncounterOption>();
        }

        public virtual IReadOnlyList<IShopEntry> GetShop()
        {
            return Array.Empty<IShopEntry>();
        }
    }
}