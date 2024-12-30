namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    public class ChoiceNode
    {
        public readonly string NodeName;
        public readonly List<ChoiceNodeOption> Options = new List<ChoiceNodeOption>();

        public ChoiceNode(string nodeName, List<ChoiceNodeOption> options)
        {
            this.NodeName = nodeName;
            this.Options = options;
        }
    }
}