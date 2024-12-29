namespace SFDDCards.ImportModels
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Threading;
    using UnityEngine;
    using SpaceDeck.UX.AssetLookup;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.Models.Imports;

    [Serializable]
    public class ElementImport : Importable
    {
        public string Name;

        [NonSerialized]
        [Obsolete("Should fetch from " + nameof(SpriteLookup))]
        public Sprite NormalArt;

        [NonSerialized]
        [Obsolete("Should fetch from " + nameof(SpriteLookup))]
        public Sprite GreyscaleArt;

        public override async Task ProcessAdditionalFilesAsync(SynchronizationContext mainThreadContext)
        {
            string normalArtFile = this.FilePath.ToLower().Replace("elementimport", "png");

            if (File.Exists(normalArtFile))
            {
                // TODO: PHASE OUT
                this.NormalArt = await ImportHelper.GetSpriteAsync(normalArtFile, 64, 64, mainThreadContext).ConfigureAwait(false);
                // PHASE IN
                SpriteLookup.SetSprite(this.Id, this.NormalArt);
            }

            string greyscaleFile = new FileInfo(this.FilePath).Extension.ToLower().Replace("elementimport", "greyscale.png");

            if (File.Exists(greyscaleFile))
            {
                // TODO: PHASE OUT
                this.GreyscaleArt = await ImportHelper.GetSpriteAsync(greyscaleFile, 64, 64, mainThreadContext).ConfigureAwait(false);
                // PHASE IN
                SpriteLookup.SetSprite(this.Id, this.GreyscaleArt, WellknownSprites.GreyScale);
            }
        }

        public override void ProcessAdditionalFiles()
        {
            string normalArtFile = this.FilePath.ToLower().Replace("elementimport", "png");

            if (File.Exists(normalArtFile))
            {
                // TODO: PHASE OUT
                this.NormalArt = ImportHelper.GetSprite(normalArtFile, 64, 64);
                // PHASE IN
                SpriteLookup.SetSprite(this.Id, this.NormalArt);
            }

            string greyscaleFile = new FileInfo(this.FilePath).Extension.ToLower().Replace("elementimport", "greyscale.png");

            if (File.Exists(greyscaleFile))
            {
                // TODO: PHASE OUT
                this.GreyscaleArt = ImportHelper.GetSprite(greyscaleFile, 64, 64);
                // PHASE IN
                SpriteLookup.SetSprite(this.Id, this.GreyscaleArt, WellknownSprites.GreyScale);
            }
        }
    }
}