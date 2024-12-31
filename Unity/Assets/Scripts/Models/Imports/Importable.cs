namespace SpaceDeck.Models.Imports
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Utility.Minimum;

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

        public QualitiesHolder GetQualities(List<StringQualityImport> stringImports, List<NumericQualityImport> numericImports)
        {
            QualitiesHolder qualitiesHolder = new QualitiesHolder();

            foreach (StringQualityImport stringImport in stringImports)
            {
                qualitiesHolder.SetStringQuality(stringImport.Key, stringImport.Value);
            }

            foreach (NumericQualityImport numericImport in numericImports)
            {
                qualitiesHolder.SetNumericQuality(numericImport.Key, numericImport.Value);
            }

            return qualitiesHolder;
        }
    }
}