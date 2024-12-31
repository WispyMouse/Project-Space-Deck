namespace SFDDCards.ImportModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Threading;
    using UnityEngine;
    using SpaceDeck.UX.AssetLookup;
    using SpaceDeck.Models.Imports;

    [Obsolete("Should transition to SpaceDeck.Models.Imports")]
    [Serializable]
    public class StatusEffectImport : Importable
    {
        public enum StatusEffectPersistence
        {
            Combat = 1,
            Campaign = 2
        }

        public string Name;
        public List<EffectOnProcImport> Effects = new List<EffectOnProcImport>();
        public StatusEffectPersistence Persistence = StatusEffectPersistence.Combat;
        public HashSet<string> Tags = new HashSet<string>();

        [NonSerialized]
        [Obsolete("Should fetch from " + nameof(SpriteLookup))]
        public Sprite StatusEffectArt;

        public override async Task ProcessAdditionalFilesAsync(SynchronizationContext mainThreadContext)
        {
            string spriteFile = this.FilePath.ToLower().Replace("statusimport", "png");

            if (File.Exists(spriteFile))
            {
                // TODO: PHASE OUT
                this.StatusEffectArt = await ImportHelper.GetSpriteAsync(spriteFile, 64, 64, mainThreadContext).ConfigureAwait(false);
                // PHASE IN
                SpriteLookup.SetSprite(this.Id, this.StatusEffectArt);
            }
        }

        public override void ProcessAdditionalFiles()
        {
            string spriteFile = this.FilePath.ToLower().Replace("statusimport", "png");

            if (File.Exists(spriteFile))
            {
                // TODO: PHASE OUT
                this.StatusEffectArt = ImportHelper.GetSprite(spriteFile, 64, 64);
                // PHASE IN
                SpriteLookup.SetSprite(this.Id, this.StatusEffectArt);
            }
        }
    }
}