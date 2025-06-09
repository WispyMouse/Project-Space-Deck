namespace SpaceDeck.Models.Instances
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class EncounterInstance
    {
        public readonly EncounterPrototype Prototype;
        public readonly IReadOnlyList<LinkedShopEntry> ShopEntries;

        public LowercaseString EncounterId;
        public string EncounterName;
        public string EncounterDescription;
        public readonly List<Entity> EncounterEntities = new List<Entity>();
        public Dictionary<CardInstance, LowercaseString> CardToZone = new Dictionary<CardInstance, LowercaseString>();
        public Dictionary<LowercaseString, List<CardInstance>> ZoneToCards = new Dictionary<LowercaseString, List<CardInstance>>();
        public readonly List<PickReward> EncounterRewards = new List<PickReward>();

        public bool HasEncounterDialogue =>
            // If there's no prototype, there's no dialogue
            !(this.Prototype == null)
            // If there is a prototype and it has encounter scripts, it is an encounter
            && (this.Prototype.EncounterScripts.Count > 0);
        public bool IsShopEncounter => 
            // By default this is a shop encounter if it contains a shop entry            
            this.ShopEntries.Count > 0 
            // If this has a prototype: If that prototype isn't a shop encounter, this isn't a shop encounter
            && (this.Prototype == null || this.Prototype.IsShopEncounter);

        private EncounterInstance(LowercaseString encounterId, string encounterName, string encounterDescription) : base()
        {
            this.EncounterId = encounterId;
            this.EncounterName = encounterName;
            this.EncounterDescription = encounterDescription;
        }

        public EncounterInstance(EncounterPrototype prototype, IEncounterEntitiesProvider entityProvider, ILinkedPickRewardProvider rewardProvider) : this(prototype.Id, prototype.Name, prototype.Description)
        {
            this.Prototype = prototype;

            this.EncounterEntities.AddRange(entityProvider.GetEntities(this.Prototype.EnemiesInEncounterById));
            this.EncounterRewards.AddRange(rewardProvider.GetRewards(this.Prototype.Rewards));

            List<LinkedShopEntry> shopEntries = new List<LinkedShopEntry>();
            foreach (LowercaseStringSet shopItem in prototype.ShopItems)
            {
                LinkedShopEntry shopEntry = rewardProvider.GetShopEntry(shopItem);
                shopEntries.Add(shopEntry);
            }

            this.ShopEntries = shopEntries;

            if (!this.IsShopEncounter && !this.HasEncounterDialogue && this.EncounterEntities.Count == 0)
            {
                Logging.DebugLog(WellknownLoggingLevels.Warning, WellknownLoggingCategories.DatabaseImportCompletion, $"Encounter with prototype id '{this.EncounterId}' is labeled as a combat encounter, but has no entities in it.");
            }
        }

        public EncounterInstance(LowercaseString id, string name, string description, IEnumerable<Entity> encounterEntities = null, IEnumerable<LinkedPickReward> rewards = null, IEnumerable<LinkedShopEntry> shopEntries = null) : this(id, name, description)
        {
            this.EncounterEntities.AddRange(encounterEntities ?? Array.Empty<Entity>());
            this.EncounterRewards.AddRange(rewards ?? Array.Empty<LinkedPickReward>());
            this.ShopEntries = new List<LinkedShopEntry>(shopEntries ?? Array.Empty<LinkedShopEntry>());

            if (!this.IsShopEncounter && !this.HasEncounterDialogue && this.EncounterEntities.Count == 0)
            {
                Logging.DebugLog(WellknownLoggingLevels.Warning, WellknownLoggingCategories.DatabaseImportCompletion, $"Encounter with prototype id '{this.EncounterId}' is labeled as a combat encounter, but has no entities in it.");
            }
        }

        public string BuildEncounterDialogue(LowercaseString index, IGameStateMutator mutator)
        {
            if (!this.Prototype.EncounterScripts.TryGetValue(index, out EncounterScript script))
            {
                return String.Empty;
            }

            StringBuilder dialogue = new StringBuilder();

            foreach (EncounterDialogueSegment dialogueSegment in script.DialogueParts)
            {
                // TODO: Check requirements
                if (true)
                {
                    dialogue.AppendLine(dialogueSegment.Dialogue);
                }
            }

            return dialogue.ToString();
        }

        public void MoveCard(CardInstance card, LowercaseString zone)
        {
            if (!this.ZoneToCards.ContainsKey(zone))
            {
                this.ZoneToCards.Add(zone, new List<CardInstance>());
            }

            if (this.CardToZone.ContainsKey(card))
            {
                // This is already tracked, so we should remove it from its previous zone
                LowercaseString previousZone = this.CardToZone[card];
                this.ZoneToCards[previousZone].Remove(card);
                this.CardToZone[card] = zone;
                this.ZoneToCards[zone].Add(card);
            }
            else
            {
                // This isn't tracked, so we need to consider adding it to lists
                this.CardToZone.Add(card, zone);
                this.ZoneToCards[zone].Add(card);
            }
        }

        public LowercaseString GetCardZone(CardInstance card)
        {
            if (this.CardToZone.TryGetValue(card, out LowercaseString zone))
            {
                return zone;
            }

            return string.Empty;
        }

        public IReadOnlyList<CardInstance> GetZoneCards(LowercaseString zone)
        {
            if (this.ZoneToCards.TryGetValue(zone, out List<CardInstance> cards))
            {
                return cards;
            }
            this.ZoneToCards.Add(zone, new List<CardInstance>());
            return this.GetZoneCards(zone);
        }

        public virtual IReadOnlyList<EncounterOption> GetOptions(LowercaseString index, IGameStateMutator mutator)
        {
            // TODO: EncounterOptions
            return Array.Empty<EncounterOption>();
        }

        public virtual LowercaseString GetStartingCampaignState()
        {
            if (this.IsShopEncounter)
            {
                return WellknownCampaignStates.ShopEncounter;
            }
            else if (this.HasEncounterDialogue)
            {
                return WellknownCampaignStates.DialogueEncounter;
            }
            else
            {
                return WellknownCampaignStates.CombatEncounter;
            }
        }
    }
}