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
    }
}