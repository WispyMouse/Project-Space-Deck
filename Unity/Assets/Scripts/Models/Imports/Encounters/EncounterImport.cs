namespace SpaceDeck.Models.Imports
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    [System.Serializable]
    public class EncounterImport : Importable
    {
        public string Name;
        public string Description;
        public string[] Tags = Array.Empty<string>();
        public string[] EnemiesInEncounterById;
        public bool IsShopEncounter;
        public string[] Arguments = Array.Empty<string>();
        // TODO: RewardImport
        public EncounterScriptImport[] DialogueScripts;

        public EncounterPrototype GetPrototype()
        {
            HashSet<LowercaseString> hashTags = new HashSet<LowercaseString>();
            foreach (string tag in (IEnumerable<string>)(this.Tags ?? Array.Empty<string>()))
            {
                hashTags.Add(tag);
            }

            List<LowercaseString> enemiesInEncounter = new List<LowercaseString>();
            foreach (string enemy in (IEnumerable<string>)(this.EnemiesInEncounterById ?? Array.Empty<string>()))
            {
                enemiesInEncounter.Add(enemy);
            }

            List<EncounterScript> encounterScripts = new List<EncounterScript>();
            foreach (EncounterScriptImport import in (IEnumerable<EncounterScriptImport>)(this.DialogueScripts ?? Array.Empty<EncounterScriptImport>()))
            {
                encounterScripts.Add(import.GetLinkedScript());
            }

            return new EncounterPrototype(this.Id,
                this.Name,
                this.Description,
                hashTags,
                enemiesInEncounter,
                this.IsShopEncounter,
                this.Arguments,
                // TODO RewardImport
                encounterScripts);
        }
    }
}