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
    public class CardImport : Importable
    {
        public string Name;
        public string EffectScript;
        public HashSet<string> Tags = new HashSet<string>();
        public List<ResourceGainImport> ElementGain = new List<ResourceGainImport>();

        [System.NonSerialized]
        [Obsolete("Should fetch from " + nameof(SpriteLookup))]
        public Sprite CardArt;

        public bool MeetsAllTags(IEnumerable<string> tags)
        {
            foreach (string tag in tags)
            {
                if (tag.ToLower() == "card")
                {
                    continue;
                }

                if (!this.Tags.Contains(tag))
                {
                    return false;
                }
            }

            return true;
        }

        public override async Task ProcessAdditionalFilesAsync(SynchronizationContext mainThreadContext)
        {
            string spriteFile = this.FilePath.ToLower().Replace("cardimport", "png");

            if (File.Exists(spriteFile))
            {
                // TODO: PHASE OUT
                this.CardArt = await ImportHelper.GetSpriteAsync(spriteFile, 144, 80, mainThreadContext).ConfigureAwait(false);
                // PHASE IN
                SpriteLookup.SetSprite(this.Id, this.CardArt);
            }
        }

        public override void ProcessAdditionalFiles()
        {
            string spriteFile = this.FilePath.ToLower().Replace("cardimport", "png");

            if (File.Exists(spriteFile))
            {
                // TODO: PHASE OUT
                this.CardArt = ImportHelper.GetSprite(spriteFile, 144, 80);
                // PHASE IN
                SpriteLookup.SetSprite(this.Id, this.CardArt);
            }
        }
    }
}