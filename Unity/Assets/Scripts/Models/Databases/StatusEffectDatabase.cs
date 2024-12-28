namespace SpaceDeck.Models.Databases
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;

    public delegate AppliedStatusEffect GetLinkedAppliedStatusEffectFactoryMethod(StatusEffectPrototype prototype);

    public static class StatusEffectDatabase
    {
        private readonly static Dictionary<LowercaseString, StatusEffectPrototype> Prototypes = new Dictionary<LowercaseString, StatusEffectPrototype>();

        private readonly static Dictionary<StatusEffectPrototype, GetLinkedAppliedStatusEffectFactoryMethod> SpecialFactories = new Dictionary<StatusEffectPrototype, GetLinkedAppliedStatusEffectFactoryMethod>();

        public static void RegisterStatusEffectPrototype(StatusEffectPrototype prototype)
        {
            if (!Prototypes.ContainsKey(prototype.Id))
            {
                Prototypes.Add(prototype.Id, prototype);
            }
        }

        public static void RegisterStatusEffectPrototypeFactory(StatusEffectPrototype prototype, GetLinkedAppliedStatusEffectFactoryMethod factoryMethod)
        {
            RegisterStatusEffectPrototype(prototype);

            if (SpecialFactories.ContainsKey(prototype))
            {
                SpecialFactories.Remove(prototype);
            }

            SpecialFactories.Add(prototype, factoryMethod);
        }

        public static AppliedStatusEffect GetInstance(LowercaseString id)
        {
            StatusEffectPrototype prototype = Prototypes[id];

            if (SpecialFactories.TryGetValue(prototype, out GetLinkedAppliedStatusEffectFactoryMethod factoryMethod))
            {
                return factoryMethod(prototype);
            }

            return new LinkedAppliedStatusEffect(Prototypes[id]);
        }

        public static void ClearDatabase()
        {
            Prototypes.Clear();
            SpecialFactories.Clear();
        }

        public static void LinkTokens()
        {
            foreach (StatusEffectPrototype curPrototype in Prototypes.Values)
            {
                if (curPrototype.Reactors != null)
                {
                    foreach(Reactor curReactor in curPrototype.Reactors)
                    {
                        if (!curReactor.LinkedTokens.HasValue && curReactor.ParsedTokens.HasValue)
                        {
                            if (LinkedTokenMaker.TryGetLinkedTokenList(curReactor.ParsedTokens.Value, out LinkedTokenList linkedTokens))
                            {
                                curReactor.LinkedTokens = linkedTokens;
                            }
                        }
                    }
                }
            }
        }
    }
}