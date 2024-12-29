namespace SFDDCards
{
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ChoiceNode
    {
        public readonly ChoiceNodeImport BasedOn;
        public readonly List<ChoiceNodeOption> Options = new List<ChoiceNodeOption>();

        public ChoiceNode(ChoiceNodeImport basedOn, RandomDecider<EncounterPrototype> decider)
        {
            this.BasedOn = basedOn;

            foreach (ChoiceNodeOptionImport choiceNodeOptionImport in basedOn.Options)
            {
                this.Options.Add(new ChoiceNodeOption(choiceNodeOptionImport, decider));
            }
        }
    }
}