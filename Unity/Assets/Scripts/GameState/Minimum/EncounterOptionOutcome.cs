namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class EncounterOptionOutcome : IResolve
    {
        public readonly LowercaseString RequirementScript;
        public readonly string Dialogue;
        public readonly LowercaseString ExecutionScript;

        public EncounterOptionOutcome(LowercaseString requirementScript, string dialogue, LowercaseString executionScript)
        {
            this.RequirementScript = requirementScript;
            this.Dialogue = dialogue;
            this.ExecutionScript = executionScript;
        }

        public void Apply(IGameStateMutator mutator)
        {
            this.Apply(mutator, out _);
        }

        public void Apply(IGameStateMutator mutator, out LowercaseString destination)
        {
            throw new System.NotImplementedException();
        }
    }
}