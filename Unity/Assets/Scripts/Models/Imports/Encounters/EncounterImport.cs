namespace SpaceDeck.Models.Imports
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
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
        public string[] RewardIdentities = Array.Empty<string>();

        public EncounterPrototype GetPrototype(IPickRewardPrototypeProvider pickRewardPrototypeProvider)
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

            List<LowercaseString> rewardIdentities = new List<LowercaseString>();
            foreach (string rewardIdentity in this.RewardIdentities)
            {
                rewardIdentities.Add(rewardIdentity);
            }

            IEnumerable<PickRewardPrototype> rewardPrototypes = pickRewardPrototypeProvider.GetPrototypes(rewardIdentities);

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