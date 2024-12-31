namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.UX;

    public abstract class StatusEffectUXHolder : MonoBehaviour
    {
        [SerializeReference]
        private StatusEffectUX StatusEffectUXPrefab;
        private Dictionary<AppliedStatusEffect, StatusEffectUX> StatusEffectLookup { get; set; } = new Dictionary<AppliedStatusEffect, StatusEffectUX>();

        public void SetStatusEffects(IEnumerable<AppliedStatusEffect> appliedEffects, Action<AppliedStatusEffect> onClickedEvent = null)
        {
            List<AppliedStatusEffect> noLongerApplicableEffects = new List<AppliedStatusEffect>(this.StatusEffectLookup.Keys);

            foreach (AppliedStatusEffect effect in appliedEffects)
            {
                if (this.StatusEffectLookup.TryGetValue(effect, out StatusEffectUX existingUX))
                {
                    existingUX.SetStacks((int)effect.Qualities.GetNumericQuality(WellknownQualities.Stacks, 0));
                }
                else
                {
                    StatusEffectUX newUX = Instantiate(this.StatusEffectUXPrefab, this.transform);
                    this.StatusEffectLookup.Add(effect, newUX);
                    newUX.SetFromEffect(effect, onClickedEvent);
                    newUX.SetStacks((int)effect.Qualities.GetNumericQuality(WellknownQualities.Stacks, 0));
                }

                noLongerApplicableEffects.Remove(effect);
            }

            if (noLongerApplicableEffects.Count > 0)
            {
                for (int ii = noLongerApplicableEffects.Count - 1; ii >= 0; ii--)
                {
                    Destroy(this.StatusEffectLookup[noLongerApplicableEffects[ii]].gameObject);
                    this.StatusEffectLookup.Remove(noLongerApplicableEffects[ii]);
                }
            }
        }

        public void Annihilate()
        {
            this.StatusEffectLookup.Clear();

            for (int ii = this.transform.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.transform.GetChild(ii).gameObject);
            }
        }
    }
}