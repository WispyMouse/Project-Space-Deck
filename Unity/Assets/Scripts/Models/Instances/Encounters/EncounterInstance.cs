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

        public override bool HasEncounterDialogue =>
            // If there's no prototype, there's no dialogue
            !(this.Prototype == null)
            // If there is a prototype and it has encounter scripts, it is an encounter
            && (this.Prototype.EncounterScripts.Count > 0);
        public override bool IsShopEncounter => 
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

        public EncounterInstance(EncounterState state)
        {
            this.Prototype = null;

            this.EncounterEntities.AddRange(state.EncounterEntities);
            this.EncounterRewards.AddRange(state.EncounterRewards);

            // TODO: Enable EncounterState fed shops
            this.ShopEntries = Array.Empty<LinkedShopEntry>();
        }

        public override string BuildEncounterDialogue(LowercaseString index, IGameStateMutator mutator)
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
    }
}