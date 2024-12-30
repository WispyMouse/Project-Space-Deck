namespace SpaceDeck.Models.Imports
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.UX.AssetLookup;
    using UnityEngine;

    [System.Serializable]
    public class ChoiceNodeImport
    {
        public string NodeName;
        public List<ChoiceNodeOptionImport> ChoiceNodeArguments;

        public ChoiceNode GetNode()
        {
            List<ChoiceNodeOption> nodeOptions = new List<ChoiceNodeOption>();
            foreach (ChoiceNodeOptionImport import in this.ChoiceNodeArguments)
            {
                nodeOptions.Add(import.GetOption());
            }

            return new ChoiceNode(this.NodeName, nodeOptions);
        }
    }
}