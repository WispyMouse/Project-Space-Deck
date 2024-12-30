namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    [System.Serializable]
    public class StatusEffectImport : Importable
    {
        public readonly List<ReactorImport> Reactors = new List<ReactorImport>();
        public readonly string Name;

        public StatusEffectPrototype GetPrototype()
        {
            List<Reactor> reactors = new List<Reactor>();
            foreach (ReactorImport reactor in this.Reactors)
            {
                reactors.Add(reactor.GetReactor());
            }

            return new StatusEffectPrototype(this.Id, this.Name, reactors);
        }
    }
}