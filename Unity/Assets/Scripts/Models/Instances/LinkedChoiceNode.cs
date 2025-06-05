namespace SpaceDeck.Models.Instances
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Deltas;
    using SpaceDeck.Models.Prototypes;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;

    public class LinkedChoiceNode : ChoiceNode
    {
        public readonly IReadOnlyList<LinkedChoiceNodeOption> LinkedOptions;

        public LinkedChoiceNode(ChoiceNode baseNode, IEncounterProvider encounterProvider) : base(baseNode.NodeName, baseNode.Options)
        {
            List<LinkedChoiceNodeOption> linkedOptions = new List<LinkedChoiceNodeOption>();

            foreach (ChoiceNodeOption option in baseNode.Options)
            {
                linkedOptions.Add(new LinkedChoiceNodeOption(option, encounterProvider.GetEncounter(option.WillEncounterId, option.Arguments)));
            }

            this.LinkedOptions = linkedOptions;
        }
    }
}