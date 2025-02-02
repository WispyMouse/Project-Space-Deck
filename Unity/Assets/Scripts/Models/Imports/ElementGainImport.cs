namespace SpaceDeck.Models.Imports
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.UX.AssetLookup;
    using UnityEngine;

    [System.Serializable]

    public class ElementGainImport
    {
        public string ElementId;
        public int ModAmount;
    }
}