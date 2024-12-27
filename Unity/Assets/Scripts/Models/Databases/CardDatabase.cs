namespace SpaceDeck.Models.Databases
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Utility.Minimum;

    public static class CardDatabase
    {
        private readonly static Dictionary<LowercaseString, CardPrototype> Prototypes = new Dictionary<LowercaseString, CardPrototype>();

        public static void RegisterCardPrototype(CardPrototype prototype)
        {
            Prototypes.Add(prototype.Id, prototype);
        }

        public static LinkedCardInstance GetInstance(LowercaseString id)
        {
            return new LinkedCardInstance(Prototypes[id]);
        }

        public static void ClearDatabase()
        {
            Prototypes.Clear();
        }
    }
}