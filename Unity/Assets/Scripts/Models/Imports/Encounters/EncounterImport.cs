namespace SpaceDeck.Models.Imports
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
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
        public EncounterScriptImport[] DialogueScripts;
        public PickRewardImport[] RewardIdentities = Array.Empty<PickRewardImport>();

        public EncounterPrototype GetPrototype()
        {
            HashSet<LowercaseString> hashTags = new HashSet<LowercaseString>();
            foreach (string tag in (IEnumerable<string>)(this.Tags ?? Array.Empty<string>()))
            {
                hashTags.Add(tag);
            }

            List<LowercaseStringSet> enemiesInEncounter = new List<LowercaseStringSet>();
            foreach (string enemyTags in (IEnumerable<string>)(this.EnemiesInEncounterById ?? Array.Empty<string>()))
            {
                enemiesInEncounter.Add(new LowercaseStringSet(enemyTags));
            }

            List<EncounterScript> encounterScripts = new List<EncounterScript>();
            foreach (EncounterScriptImport import in (IEnumerable<EncounterScriptImport>)(this.DialogueScripts ?? Array.Empty<EncounterScriptImport>()))
            {
                encounterScripts.Add(import.GetLinkedScript());
            }

            List<PickRewardPrototype> rewardPrototypes = new List<PickRewardPrototype>();
            foreach (PickRewardImport import in this.RewardIdentities)
            {
                rewardPrototypes.Add(import.GetPrototype());
            }

            return new EncounterPrototype(this.Id,
                this.Name,
                this.Description,
                hashTags,
                enemiesInEncounter,
                this.IsShopEncounter,
                this.Arguments,
                encounterScripts,
                rewardPrototypes);
        }
    }
}