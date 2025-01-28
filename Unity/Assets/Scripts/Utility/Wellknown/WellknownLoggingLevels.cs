namespace SpaceDeck.Utility.Wellknown
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    public static class WellknownLoggingLevels
    {
        public static readonly LowercaseString ImportComplete = nameof(ImportComplete);
        public static readonly LowercaseString Debug = nameof(Debug);
        public static readonly LowercaseString Warning = nameof(Warning);
        public static readonly LowercaseString Error = nameof(Error);
    }
}