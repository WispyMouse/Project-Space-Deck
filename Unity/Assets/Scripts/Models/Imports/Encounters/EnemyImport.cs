namespace SpaceDeck.Models.Imports
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    [System.Serializable]
    public class EnemyImport : Importable
    {
        public List<string> Tags = new List<string>();
        public string Name;
        public List<StringQualityImport> StringQualities = new List<StringQualityImport>();
        public List<NumericQualityImport> NumberQualities = new List<NumericQualityImport>();
        public List<EnemyAttackImport> EnemyAttacks = new List<EnemyAttackImport>();

        public EnemyPrototype GetPrototype()
        {
            LowercaseStringSet lowercaseSet = new LowercaseStringSet(this.Tags);

            Dictionary<LowercaseString, EnemyAttack> attacks = new Dictionary<LowercaseString, EnemyAttack>();
            foreach (EnemyAttackImport import in this.EnemyAttacks)
            {
                if (attacks.ContainsKey(import.Id))
                {
                    Logging.DebugLog(WellknownLoggingLevels.Error,
                        WellknownLoggingCategories.DatabaseImportCompletion,
                        $"Multiple attacks with the same id '{import.Id}' are present in attacks list. Skpiping ability.");
                    continue;
                }

                attacks.Add(import.Id, new EnemyAttack(import.Id, import.AttackScript));
            }

            return new EnemyPrototype(this.Id, lowercaseSet, GetQualities(this.StringQualities, this.NumberQualities), attacks);
        }
    }
}
