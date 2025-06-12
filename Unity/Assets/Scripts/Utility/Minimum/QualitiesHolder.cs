namespace SpaceDeck.Utility.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class QualitiesHolder
    {
        private readonly Dictionary<LowercaseString, decimal> NumericQualities = new Dictionary<LowercaseString, decimal>();
        private readonly Dictionary<LowercaseString, string> StringQualities = new Dictionary<LowercaseString, string>();

        public decimal GetNumericQuality(LowercaseString index, decimal defaultValue = 0)
        {
            if (this.NumericQualities.TryGetValue(index, out decimal qualityValue))
            {
                return qualityValue;
            }

            this.NumericQualities.Add(index, defaultValue);
            return defaultValue;
        }

        public bool TryGetNumericQuality(LowercaseString index, out decimal existingValue)
        {
            return this.NumericQualities.TryGetValue(index, out existingValue);
        }

        public bool TryGetStringQuality(LowercaseString index, out string existingValue)
        {
            return this.StringQualities.TryGetValue(index, out existingValue);
        }

        public string GetStringQuality(LowercaseString index, string defaultValue = "")
        {
            if (this.StringQualities.TryGetValue(index, out string qualityValue))
            {
                return qualityValue;
            }

            this.StringQualities.Add(index, defaultValue);
            return defaultValue;
        }

        public void SetNumericQuality(LowercaseString index, decimal newValue)
        {
            if (this.NumericQualities.ContainsKey(index))
            {
                this.NumericQualities[index] = newValue;
            }
            else
            {
                this.NumericQualities.Add(index, newValue);
            }
        }

        public void SetStringQuality(LowercaseString index, string newValue)
        {
            if (this.StringQualities.ContainsKey(index))
            {
                this.StringQualities[index] = newValue;
            }
            else
            {
                this.StringQualities.Add(index, newValue);
            }
        }

        public QualitiesHolder Clone()
        {
            QualitiesHolder clone = new QualitiesHolder();

            foreach (string stringKey in this.StringQualities.Keys)
            {
                clone.SetStringQuality(stringKey, this.StringQualities[stringKey]);
            }

            foreach (string numericKey in this.NumericQualities.Keys)
            {
                clone.SetNumericQuality(numericKey, this.NumericQualities[numericKey]);
            }

            return clone;
        }

        public void AddQualities(QualitiesHolder from)
        {
            foreach (string stringKey in from.StringQualities.Keys)
            {
                this.SetStringQuality(stringKey, from.StringQualities[stringKey]);
            }

            foreach (string numericKey in from.NumericQualities.Keys)
            {
                this.SetNumericQuality(numericKey, from.NumericQualities[numericKey]);
            }
        }

        public IReadOnlyDictionary<LowercaseString, decimal> GetNumericQualities()
        {
            return new Dictionary<LowercaseString, decimal>(this.NumericQualities);
        }

        public IReadOnlyDictionary<LowercaseString, string> GetStringQualities()
        {
            return new Dictionary<LowercaseString, string>(this.StringQualities);
        }
    }
}