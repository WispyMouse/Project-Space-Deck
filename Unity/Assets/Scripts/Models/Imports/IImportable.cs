namespace SpaceDeck.Models.Imports
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;

    public interface IImportable
    {
        public string FilePath { get; set; }
        public string Id { get; }
        public Task ProcessAdditionalFilesAsync(SynchronizationContext mainThreadContext);
        public void ProcessAdditionalFiles();
    }
}