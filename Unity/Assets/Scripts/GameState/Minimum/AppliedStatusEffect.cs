namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    public class AppliedStatusEffect : IHaveQualities
    {
        public readonly LowercaseString Id;

        public QualitiesHolder Qualities => this._Qualities;
        private readonly QualitiesHolder _Qualities = new QualitiesHolder();

        public readonly HashSet<LowercaseString> TriggerOnEventIds = new HashSet<LowercaseString>();

        public AppliedStatusEffect(LowercaseString id)
        {
            this.Id = id;
        }

        public virtual bool TryApplyStatusEffect(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            applications = null;
            return false;
        }
    }
}