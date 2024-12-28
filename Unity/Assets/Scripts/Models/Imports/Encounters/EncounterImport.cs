namespace SpaceDeck.Models.Imports
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Models.Prototypes;

    [System.Serializable]
    public class EncounterImport
    {
        public string Id;
        public string Name;
        public string Description;
        public string[] Tags = Array.Empty<string>();
        public string EnemiesInEncounterById;
        public bool IsShopEncounter;
        public string[] Arguments = Array.Empty<string>();
        // TODO: RewardImport
        public EncounterScriptImport[] DialogueScripts;

        public EncounterPrototype GetPrototype()
        {
            return new EncounterPrototype(this.Id);
        }
    }
}