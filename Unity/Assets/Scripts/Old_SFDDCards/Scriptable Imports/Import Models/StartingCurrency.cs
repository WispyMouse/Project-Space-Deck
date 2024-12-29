namespace SFDDCards.ImportModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;

    [Serializable]

    public class StartingCurrency
    {
        public string CurrencyId;
        public int StartingAmount;
    }
}