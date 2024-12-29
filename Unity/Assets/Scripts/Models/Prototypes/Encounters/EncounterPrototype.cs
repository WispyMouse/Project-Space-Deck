namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class EncounterPrototype
    {
        public readonly LowercaseString Id;
        public readonly string Name;
        public readonly string Description;
        public readonly HashSet<LowercaseString> EncounterTags = new HashSet<LowercaseString>();
        public readonly IReadOnlyList<LowercaseString> EnemiesInEncounterById = new List<LowercaseString>();
        public readonly bool IsShopEncounter;
        public readonly IReadOnlyList<string> Arguments = new List<string>();
        // TODO RewardImport
        public readonly Dictionary<LowercaseString, EncounterScript> EncounterScripts = new Dictionary<LowercaseString, EncounterScript>();

        public EncounterPrototype(
            LowercaseString id,
            string name,
            string description,
            HashSet<LowercaseString> encounterTags,
            IReadOnlyList<LowercaseString> enemiesInEncounterById,
            bool isShopEncounter,
            IReadOnlyList<string> arguments,
            // TODO RewardImport
            IEnumerable<EncounterScript> encounterScripts)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.EncounterTags = encounterTags;
            this.EnemiesInEncounterById = enemiesInEncounterById;
            this.IsShopEncounter = isShopEncounter;
            this.Arguments = arguments;
            // TODO: RewardImport
            
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