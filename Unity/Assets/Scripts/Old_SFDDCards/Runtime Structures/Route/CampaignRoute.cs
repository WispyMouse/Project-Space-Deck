namespace SFDDCards
{
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CampaignRoute
    {
        public string RouteName => this.BasedOn.RouteName;
        public readonly List<ChoiceNode> Nodes = new List<ChoiceNode>();
        public readonly RouteImport BasedOn;

        public CampaignRoute(RunConfiguration runConfiguration, RouteImport basedOn)
        {
            this.BasedOn = basedOn;
            RandomDecider<EncounterPrototype> decider = new DoNotRepeatRandomDecider<EncounterPrototype>();

            foreach (ChoiceNodeImport node in basedOn.RouteNodes)
            {
                this.Nodes.Add(new ChoiceNode(node, decider));
            }
        }
    }
}