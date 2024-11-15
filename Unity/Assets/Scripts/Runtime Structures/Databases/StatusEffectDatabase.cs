namespace SFDDCards
{
    using SFDDCards.ImportModels;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public static class StatusEffectDatabase
    {
        public static Dictionary<string, StatusEffect> EffectData { get; private set; } = new Dictionary<string, StatusEffect>();

        public static void AddStatusEffectToDatabase(StatusEffectImport importData)
        {
            string lowerId = importData.Id.ToLower();

            if (EffectData.ContainsKey(lowerId))
            {
                Debug.LogError($"EffectData dictionary already contains id {lowerId}");
                return;
            }

            EffectData.Add(importData.Id.ToLower(), new StatusEffect(importData));
        }

        public static StatusEffect GetModel(string id, RandomDecider<StatusEffect> decider = null)
        {
            if (decider == null)
            {
                decider = new RandomDecider<StatusEffect>();
            }

            string lowerId = id.ToLower();

            // If there are brackets, this might be a set of tag criteria.
            Match tagMatches = Regex.Match(id, @"(?:\[(?<tag>[^]]+)\])+");
            if (tagMatches.Success)
            {
                HashSet<string> tags = new HashSet<string>();
                foreach (Capture curCapture in tagMatches.Groups[1].Captures)
                {
                    tags.Add(curCapture.Value.ToLower());
                }

                if (!TryGetStatusEffectWithAllTags(decider, tags, out StatusEffect model))
                {
                    return null;
                }

                return model;
            }
            
            if (!EffectData.TryGetValue(lowerId, out StatusEffect foundModel))
            {
                Debug.LogError($"EffectData dictionary lookup does not contain id {lowerId}");
            }

            return foundModel;
        }

        public static void ClearDatabase()
        {
            EffectData.Clear();
        }

        public static bool TryGetStatusEffectWithAllTags(RandomDecider<StatusEffect> decider, HashSet<string> tags, out StatusEffect effect)
        {
            List<StatusEffect> candidates = new List<StatusEffect>();

            foreach (StatusEffect model in EffectData.Values)
            {
                if (model.MeetsAllTags(tags))
                {
                    candidates.Add(model);
                }
            }

            if (candidates.Count == 0)
            {
                effect = null;
                return false;
            }

            effect = decider.ChooseRandomly(candidates);
            return true;
        }

        public static bool TryGetStatusEffectById(string id, out StatusEffect effect)
        {
            return EffectData.TryGetValue(id.ToLower(), out effect);
        }
    }
}