namespace SFDDCards
{
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ChoiceNodeOption
    {
        public readonly ChoiceNodeOptionImport BasedOn;
        public readonly EvaluatedEncounter WillEncounter;
        public readonly EncounterInstance _WillEncounter;

        public bool WasSelected { get; set; } = false;

        public ChoiceNodeOption(ChoiceNodeOptionImport basedOn, RandomDecider<EncounterPrototype> decider)
        {
            this.BasedOn = basedOn;

            if (!EncounterDatabase.TryGetEncounterWithArguments(decider, this.BasedOn.ChoiceNodeKind, this.BasedOn.ChoiceNodeArguments, out _WillEncounter))
            {
                GlobalUpdateUX.LogTextEvent?.Invoke($"Failed to parse arguments for encounter based on {this.BasedOn.ChoiceNodeKind}.", GlobalUpdateUX.LogType.RuntimeError);
            }
        }

        public string GetName()
        {
            return this.WillEncounter.GetName();
        }

        public string GetDescription()
        {
            return this.WillEncounter.GetDescription();
        }
    }
}