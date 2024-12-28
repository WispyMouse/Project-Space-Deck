namespace SFDDCards
{
    using SFDDCards.ImportModels;
    using SpaceDeck.Models.Imports;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    [Obsolete("Should transition to " + nameof(SpaceDeck.Models.Instances.EncounterInstance))]
    public class EncounterModel
    {
        public string Id;

        public string Name;
        public string Description;
        public HashSet<string> EncounterTags { get; set; } = new HashSet<string>();

        public List<string> EnemiesInEncounterById { get; set; } = new List<string>();

        public bool IsShopEncounter;
        public List<string> Arguments { get; set; } = new List<string>();

        public RewardImport RewardsModel;

        public Dictionary<string, EncounterScriptImport> EncounterScripts = new Dictionary<string, EncounterScriptImport>();


        public EncounterModel(SFDDCards.ImportModels.EncounterImport basedOn)
        {
            HashSet<string> lowerCaseTags = new HashSet<string>();
            foreach (string tag in basedOn.Tags)
            {
                lowerCaseTags.Add(tag.ToLower());
            }

            List<string> lowerCaseEnemyIds = new List<string>();
            foreach (string enemyId in basedOn.EnemyIds)
            {
                lowerCaseEnemyIds.Add(enemyId.ToLower());
            }

            this.Id = basedOn.Id.ToLower();
            this.Name = basedOn.Name;
            this.EncounterTags = lowerCaseTags;
            this.Description = basedOn.Description;
            this.EnemiesInEncounterById = lowerCaseEnemyIds;
            this.IsShopEncounter = basedOn.IsShopEncounter;
            this.Arguments = basedOn.Arguments;

            if (basedOn.EncounterScripts != null)
            {
                foreach (EncounterScriptImport import in basedOn.EncounterScripts)
                {
                    this.EncounterScripts.Add(import.Id, import);
                }
            }

            if (!string.IsNullOrEmpty(basedOn.StandardRewardId) && RewardDatabase.TryGetReward(basedOn.StandardRewardId, out RewardImport model))
            {
                this.RewardsModel = model;
            }
            else
            {
                this.RewardsModel = basedOn.CustomReward;
            }
        }

        public List<EnemyModel> GetEnemyModels()
        {
            List<EnemyModel> models = new List<EnemyModel>();
            DoNotRepeatRandomDecider<EnemyModel> dontRepeatDecider = new DoNotRepeatRandomDecider<EnemyModel>();

            foreach (string enemyId in this.EnemiesInEncounterById)
            {
                models.Add(EnemyDatabase.GetModel(enemyId, dontRepeatDecider));
            }

            return models;
        }

        public bool MeetsAllTags(HashSet<string> tags)
        {
            return this.EncounterTags.Overlaps(tags);
        }

        public string BuildEncounterDialogue(string index, CampaignContext campaignContext)
        {
            if (!this.EncounterScripts.TryGetValue(index.ToLower(), out EncounterScriptImport script))
            {
                return String.Empty;
            }

            StringBuilder dialogue = new StringBuilder();

            foreach (EncounterDialogueSegmentImport dialogueSegment in script.DialogueParts)
            {
                if (!campaignContext.RequirementsAreMet(dialogueSegment.RequirementScript))
                {
                    continue;
                }

                dialogue.AppendLine(dialogueSegment.Dialogue);
            }

            return dialogue.ToString();
        }

        public List<EncounterOptionImport> GetOptions(string index, CampaignContext campaignContext)
        {
            if (!this.EncounterScripts.TryGetValue(index.ToLower(), out EncounterScriptImport script))
            {
                return null;
            }

            List<EncounterOptionImport> scripts = new List<EncounterOptionImport>();

            foreach (EncounterOptionImport option in script.Options)
            {
                if (!campaignContext.RequirementsAreMet(option.RequirementScript))
                {
                    continue;
                }

                scripts.Add(option);
            }

            return scripts;
        }
    }
}