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
        public List<string> Tags = new List<string>();
        public string Name;
        public List<StringQualityImport> StringQualities = new List<StringQualityImport>();
        public List<NumericQualityImport> NumberQualities = new List<NumericQualityImport>();

        public EnemyPrototype GetPrototype()
        {
            LowercaseStringSet lowercaseSet = new LowercaseStringSet(this.Tags);
            return new EnemyPrototype(this.Id, lowercaseSet, GetQualities(this.StringQualities, this.NumberQualities));
        }
    }
}
