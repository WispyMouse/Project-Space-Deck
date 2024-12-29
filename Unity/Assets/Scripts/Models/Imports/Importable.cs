namespace SpaceDeck.Models.Imports
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using SpaceDeck.Models.Imports;

    [System.Serializable]
    public abstract class Importable : IImportable
    {
        string IImportable.Id
        {
            get
            {
                return this.Id;
            }
        }

        public string Id;
        public string FilePath { get; set; }

        public virtual async Task ProcessAdditionalFilesAsync(SynchronizationContext mainThreadContext)
        {
            await Task.CompletedTask;
        }

        public virtual void ProcessAdditionalFiles()
        {
        }
    }
}