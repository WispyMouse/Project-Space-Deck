namespace SpaceDeck.Utility.Wellknown
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    public static class WellknownLoggingCategories
    {
        public static readonly LowercaseString EvaluatableEvaluation = nameof(EvaluatableEvaluation);
        public static readonly LowercaseString Test = nameof(Test);
        public static readonly LowercaseString CardImport = nameof(CardImport);

        public static readonly LowercaseString GetLinkedScriptingToken = nameof(GetLinkedScriptingToken);
        public static readonly LowercaseString TokenTryGetChanges = nameof(TokenTryGetChanges);
        public static readonly LowercaseString LinkConstructor = nameof(LinkConstructor);
        public static readonly LowercaseString ProviderEvaluation = nameof(ProviderEvaluation);

        public static readonly LowercaseString TryGetLinkedTokenList = nameof(TryGetLinkedTokenList);
        public static readonly LowercaseString TryCreateDelta = nameof(TryCreateDelta);

        public static readonly LowercaseString LinkingFailure = nameof(LinkingFailure);
        public static readonly LowercaseString ParseTokenText = nameof(ParseTokenText);
    }
}