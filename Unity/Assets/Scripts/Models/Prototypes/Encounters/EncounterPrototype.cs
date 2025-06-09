namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EncounterPrototype
    {
        public readonly LowercaseString Id;
        public readonly string Name;
        public readonly string Description;
        public readonly HashSet<LowercaseString> EncounterTags = new HashSet<LowercaseString>();
        public readonly IEnumerable<LowercaseStringSet> EnemiesInEncounterById = new List<LowercaseStringSet>();
        public bool IsShopEncounter => ShopItems.Count() != 0;
        public readonly IEnumerable<string> Arguments = new List<string>();
        public readonly Dictionary<LowercaseString, EncounterScript> EncounterScripts = new Dictionary<LowercaseString, EncounterScript>();
        public readonly IEnumerable<PickRewardPrototype> Rewards = new List<PickRewardPrototype>();
        public readonly IEnumerable<LowercaseStringSet> ShopItems = new List<LowercaseStringSet>();

        public EncounterPrototype(
            LowercaseString id,
            string name,
            string description,
            HashSet<LowercaseString> encounterTags,
            IEnumerable<LowercaseStringSet> enemiesInEncounterById,
            IEnumerable<string> arguments,
            IEnumerable<EncounterScript> encounterScripts,
            IEnumerable<PickRewardPrototype> rewards,
            IEnumerable<LowercaseStringSet> shopItems)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.EncounterTags = encounterTags ?? new HashSet<LowercaseString>();
            this.EnemiesInEncounterById = enemiesInEncounterById ?? Array.Empty<LowercaseStringSet>(); ;
            this.Arguments = arguments ?? Array.Empty<string>();
            this.Rewards = rewards ?? Array.Empty<PickRewardPrototype>();
            this.ShopItems = shopItems ?? Array.Empty<LowercaseStringSet>();
            
            foreach (EncounterScript script in encounterScripts)
            {
                this.EncounterScripts.Add(script.Id, script);
            }
        }

        public bool MeetsAllTags(HashSet<LowercaseString> tags)
        {
            return this.EncounterTags.Overlaps(tags);
        }
    }
}