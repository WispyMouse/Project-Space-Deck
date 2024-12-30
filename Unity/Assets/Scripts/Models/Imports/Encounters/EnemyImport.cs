namespace SpaceDeck.Models.Imports
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    [System.Serializable]
    public class EnemyImport : Importable
    {
        public EnemyPrototype GetPrototype()
        {
            return new EnemyPrototype(this.Id);
        }
    }
}
