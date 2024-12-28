namespace SFDDCards.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public abstract class StatusEffectUXHolder : MonoBehaviour
    {
        [SerializeReference]
        private StatusEffectUX StatusEffectUXPrefab;

        [Obsolete("Should transition to " + nameof(_StatusEffectLookup))]
        private Dictionary<SFDDCards.AppliedStatusEffect, StatusEffectUX> StatusEffectLookup { get; set; } = new Dictionary<SFDDCards.AppliedStatusEffect, StatusEffectUX>();
        private Dictionary<AppliedStatusEffect, StatusEffectUX> _StatusEffectLookup { get; set; } = new Dictionary<AppliedStatusEffect, StatusEffectUX>();

        [Obsolete("Should transition to " + nameof(_SetStatusEffects))]
        public void SetStatusEffects(List<SFDDCards.AppliedStatusEffect> appliedEffects, Action<SFDDCards.AppliedStatusEffect> onClickedEvent = null)
        {
            List<SFDDCards.AppliedStatusEffect> noLongerApplicableEffects = new List<SFDDCards.AppliedStatusEffect>(this.StatusEffectLookup.Keys);

            foreach (SFDDCards.AppliedStatusEffect effect in appliedEffects)
            {
                if (this.StatusEffectLookup.TryGetValue(effect, out StatusEffectUX existingUX))
                {
                    existingUX.SetStacks(effect.Stacks);
                }
                else
                {
                    StatusEffectUX newUX = Instantiate(this.StatusEffectUXPrefab, this.transform);
                    this.StatusEffectLookup.Add(effect, newUX);
                    newUX.SetFromEffect(effect, onClickedEvent);
                    newUX.SetStacks(effect.Stacks);
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

        public void _SetStatusEffects(IEnumerable<AppliedStatusEffect> appliedEffects, Action<AppliedStatusEffect> onClickedEvent = null)
        {
            List<AppliedStatusEffect> noLongerApplicableEffects = new List<AppliedStatusEffect>(this._StatusEffectLookup.Keys);

            foreach (AppliedStatusEffect effect in appliedEffects)
            {
                if (this._StatusEffectLookup.TryGetValue(effect, out StatusEffectUX existingUX))
                {
                    existingUX.SetStacks((int)effect.Qualities.GetNumericQuality(WellknownQualities.Stacks, 0));
                }
                else
                {
                    StatusEffectUX newUX = Instantiate(this.StatusEffectUXPrefab, this.transform);
                    this._StatusEffectLookup.Add(effect, newUX);
                    newUX._SetFromEffect(effect, onClickedEvent);
                    newUX.SetStacks((int)effect.Qualities.GetNumericQuality(WellknownQualities.Stacks, 0));
                }

                noLongerApplicableEffects.Remove(effect);
            }

            if (noLongerApplicableEffects.Count > 0)
            {
                for (int ii = noLongerApplicableEffects.Count - 1; ii >= 0; ii--)
                {
                    Destroy(this._StatusEffectLookup[noLongerApplicableEffects[ii]].gameObject);
                    this._StatusEffectLookup.Remove(noLongerApplicableEffects[ii]);
                }
            }
        }

        public void Annihilate()
        {
            this.StatusEffectLookup.Clear();
            this._StatusEffectLookup.Clear();

            for (int ii = this.transform.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.transform.GetChild(ii).gameObject);
            }
        }
    }
}