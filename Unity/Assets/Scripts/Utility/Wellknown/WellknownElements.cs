namespace SpaceDeck.Utility.Wellknown
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    public static class WellknownElements
    {
        public static readonly LowercaseString Gain = nameof(Gain);

        public static LowercaseString GetElementGain(LowercaseString elementId)
        {
            return Gain + elementId;
        }
    }
}