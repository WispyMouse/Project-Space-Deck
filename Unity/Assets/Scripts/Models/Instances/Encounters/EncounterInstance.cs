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

    public class EncounterInstance : EncounterState
    {
        public readonly EncounterPrototype Prototype;
        public readonly IReadOnlyList<LinkedShopEntry> ShopEntries;

        public override bool HasEncounterDialogue => this.Prototype.EncounterScripts.Count > 0;
        public override bool IsShopEncounter => this.Prototype.IsShopEncounter;

        public EncounterInstance(EncounterPrototype prototype, IEncounterEntitiesProvider entityProvider, ILinkedPickRewardProvider rewardProvider) :base()
        {
            this.Prototype = prototype;

            this.EncounterId = prototype.Id;
            this.EncounterName = prototype.Name;
            this.EncounterDescription = prototype.Description;
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

            this.EncounterId = state.EncounterId;
            this.EncounterName = state.EncounterName;
            this.EncounterDescription = state.EncounterDescription;
            this.EncounterEntities.AddRange(state.EncounterEntities);
            this.EncounterRewards.AddRange(state.EncounterRewards);
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