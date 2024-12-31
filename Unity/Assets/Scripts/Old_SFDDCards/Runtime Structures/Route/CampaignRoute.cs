namespace SFDDCards
{
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Obsolete("Transition to SpaceDeck.GameState.Minimum.Route")]
    public class CampaignRoute
    {
        public string RouteName => this.BasedOn.RouteName;

        public readonly List<ChoiceNode> Nodes = new List<ChoiceNode>();

        [System.Obsolete("Transition to " + nameof(_BasedOn))]
        public readonly RouteImport BasedOn;
        public readonly SpaceDeck.GameState.Minimum.Route _BasedOn;

        [System.Obsolete("Transition to constructor with new data types")]
        public CampaignRoute(RunConfiguration runConfiguration, RouteImport basedOn)
        {
            this.BasedOn = basedOn;
            RandomDecider<EncounterPrototype> decider = new DoNotRepeatRandomDecider<EncounterPrototype>();

            foreach (ChoiceNodeImport node in basedOn.RouteNodes)
            {
                this.Nodes.Add(new ChoiceNode(node, decider));
            }
        }

        public CampaignRoute(RunConfiguration runConfiguration, SpaceDeck.GameState.Minimum.Route basedOn)
        {
            throw new System.NotImplementedException();
        }
    }
}