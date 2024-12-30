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
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.UX.AssetLookup;
    using UnityEngine;

    [System.Serializable]
    public class ElementImport : Importable
    {
        public string Name;

        public Currency GetCurrency()
        {
            return new Currency(this.Id, this.Name);
        }

        public override async Task ProcessAdditionalFilesAsync(SynchronizationContext mainThreadContext)
        {
            string normalArtFile = this.FilePath.ToLower().Replace("elementimport", "png");

            if (File.Exists(normalArtFile))
            {
                Sprite art = await ImportHelper.GetSpriteAsync(normalArtFile, 64, 64, mainThreadContext).ConfigureAwait(false);
                SpriteLookup.SetSprite(this.Id, art);
            }

            string greyscaleFile = new FileInfo(this.FilePath).Extension.ToLower().Replace("elementimport", "greyscale.png");

            if (File.Exists(greyscaleFile))
            {
                Sprite greyscaleArt = await ImportHelper.GetSpriteAsync(greyscaleFile, 64, 64, mainThreadContext).ConfigureAwait(false);
                SpriteLookup.SetSprite(this.Id, greyscaleArt, WellknownSprites.GreyScale);
            }
        }

        public override void ProcessAdditionalFiles()
        {
            string normalArtFile = this.FilePath.ToLower().Replace("elementimport", "png");

            if (File.Exists(normalArtFile))
            {
                Sprite art = ImportHelper.GetSprite(normalArtFile, 64, 64);
                SpriteLookup.SetSprite(this.Id, art);
            }

            string greyscaleFile = new FileInfo(this.FilePath).Extension.ToLower().Replace("elementimport", "greyscale.png");

            if (File.Exists(greyscaleFile))
            {
                Sprite greyscaleArt = ImportHelper.GetSprite(greyscaleFile, 64, 64);
                SpriteLookup.SetSprite(this.Id, greyscaleArt, WellknownSprites.GreyScale);
            }
        }
    }
}