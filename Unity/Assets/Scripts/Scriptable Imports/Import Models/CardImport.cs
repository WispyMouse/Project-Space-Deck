namespace SFDDCards.ImportModels
{
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
                this.CardArt = await ImportHelper.GetSpriteAsync(spriteFile, 144, 80, mainThreadContext).ConfigureAwait(false);
            }
        }

        public override void ProcessAdditionalFiles()
        {
            string spriteFile = this.FilePath.ToLower().Replace("cardimport", "png");

            if (File.Exists(spriteFile))
            {
                this.CardArt = ImportHelper.GetSprite(spriteFile, 144, 80);
            }
        }
    }
}