namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    public static class EncounterDatabase
    {
        public static Dictionary<LowercaseString, EncounterPrototype> EncounterData { get; set; } = new Dictionary<LowercaseString, EncounterPrototype>();

        public static void AddEncounter(EncounterImport import)
        {
            AddEncounter(import);
        }

        public static void AddEncounter(EncounterPrototype toAdd)
        {
            EncounterData.Add(toAdd.Id, toAdd);
        }

        public static bool TryGetEncounterWithArguments(RandomDecider<EncounterPrototype> decider, string kind, List<string> arguments, out EncounterInstance encounter)
        {
            kind = kind.ToLower();

            // If there are brackets, this might be a set of tag criteria.
            Match tagMatches = Regex.Match(kind, @"\[([^]]+)\]");
            if (tagMatches.Success)
            {
                HashSet<LowercaseString> tags = new HashSet<LowercaseString>();
                foreach (Capture curCapture in tagMatches.Groups[1].Captures)
                {
                    tags.Add(curCapture.Value.ToLower());
                }
                
                if (!TryGetEncounterWithAllTags(decider, tags, out EncounterPrototype model))
                {
                    encounter = null;
                    return false;
                }

                encounter = GetEvaluatorForKind(model);
                return true;
            }
            else
            {
                if (!EncounterData.TryGetValue(kind, out EncounterPrototype encounterModel))
                {
                    encounter = null;
                    return false;
                }

                encounter = GetEvaluatorForKind(encounterModel);
                return true;
            }
        }

        public static bool TryGetEncounterWithAllTags(RandomDecider<EncounterPrototype> decider, HashSet<LowercaseString> tags, out EncounterPrototype encounter)
        {
            if (decider == null)
            {
                decider = new RandomDecider<EncounterPrototype>();
            }

            List<EncounterPrototype> candidates = new List<EncounterPrototype>();
            string singletag = string.Empty;
            if (tags.Count == 1)
            {
                singletag = new List<LowercaseString>(tags)[0];
            }

            foreach (EncounterPrototype model in EncounterData.Values)
            {
                // If there is only a single tag, and it matches this model's Id, we've found the encounter
                if (singletag == model.Id)
                {
                    encounter = model;
                    return true;
                }
                // Otherwise if every tag provided is within the model's tags, use this
                else if (model.MeetsAllTags(tags))
                {
                    candidates.Add(model);
                }
            }

            if (candidates.Count == 0)
            {
                encounter = null;
                return false;
            }

            encounter = decider.ChooseRandomly(candidates);
            
            return true;
        }

        public static EncounterInstance GetEvaluatorForKind(EncounterPrototype model)
        {
            return new EncounterInstance(model, EncounterDatabaseEntitiesProvider.Instance, RewardDatabasePickRewardProvider.Instance);
        }

        public static void ClearDatabase()
        {
            EncounterData.Clear();
        }
    }
}