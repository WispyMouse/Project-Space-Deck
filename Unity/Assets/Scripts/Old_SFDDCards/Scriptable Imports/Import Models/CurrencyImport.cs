namespace SFDDCards.ImportModels
{
    using SpaceDeck.Models.Imports;
    using SpaceDeck.UX.AssetLookup;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;

    [Serializable]
    public class CurrencyImport : Importable
    {
        public string Name;

        [System.NonSerialized]
        [Obsolete("Should fetch from " + nameof(SpriteLookup))]
        public Sprite CurrencyArt;

        [System.NonSerialized]
        public int? SpriteIndex;

        public string GetNameOrIcon()
        {
            if (this.SpriteIndex.HasValue)
            {
                return $"<sprite index={this.SpriteIndex.Value}>";
            }

            return this.Name;
        }

        public string GetNameAndMaybeIcon()
        {
            if (this.SpriteIndex.HasValue)
            {
                return $"<sprite index={this.SpriteIndex.Value}>{this.Name}";
            }

            return this.Name;
        }

        public override async Task ProcessAdditionalFilesAsync(SynchronizationContext mainThreadContext)
        {
            string spriteFile = this.FilePath.ToLower().Replace("currencyimport", "png");

            if (File.Exists(spriteFile))
            {
                // TODO: PHASE OUT
                this.CurrencyArt = await ImportHelper.GetSpriteAsync(spriteFile, 64, 64, mainThreadContext).ConfigureAwait(false);
                // PHASE IN
                SpriteLookup.SetSprite(this.Id, this.CurrencyArt);
            }
        }

        public override void ProcessAdditionalFiles()
        {
            string spriteFile = this.FilePath.ToLower().Replace("currencyimport", "png");

            if (File.Exists(spriteFile))
            {
                // TODO: PHASE OUT
                this.CurrencyArt = ImportHelper.GetSprite(spriteFile, 64, 64);
                // PHASE IN
                SpriteLookup.SetSprite(this.Id, this.CurrencyArt);
            }
        }
    }
}