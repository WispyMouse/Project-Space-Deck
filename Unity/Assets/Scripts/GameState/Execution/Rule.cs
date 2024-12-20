namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    public abstract class Rule
    {
        /// <summary>
        /// When an event with one of these ids happens, the rule will try to apply.
        /// If this list is empty, then the rule applies *constantly*, whenever *anything* happens.
        /// </summary>
        public readonly HashSet<LowercaseString> TriggerOnEventIds = new HashSet<LowercaseString>();

        public Rule()
        {

        }

        public Rule(HashSet<LowercaseString> triggerOnEventIds)
        {
            this.TriggerOnEventIds = triggerOnEventIds;
        }

        public Rule(LowercaseString triggerOneventId) : this(new HashSet<LowercaseString>() { triggerOneventId })
        {

        }

        public virtual bool TryApplyRule(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            applications = null;
            return false;
        }
    }
}