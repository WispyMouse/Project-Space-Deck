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
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.UX.AssetLookup;
    using UnityEngine;

    [System.Serializable]
    public class CardImport : Importable
    {
        public string Name;
        public string EffectScript;

        public CardPrototype GetPrototype()
        {
            if (!TokenTextMaker.TryGetTokenTextFromString(this.EffectScript, out TokenText statement))
            {
                // TODO: LOG IMPORT FAILURE
                return null;
            }
            else if (!ParsedTokenMaker.TryGetParsedTokensFromTokenText(statement, out ParsedTokenList parsedTokens))
            {
                // TODO: LOG IMPORT FAILURE
                return null;
            }
            else
            {
                return new CardPrototype(this.Id, parsedTokens);
            }
        }

        public override async Task ProcessAdditionalFilesAsync(SynchronizationContext mainThreadContext)
        {
            string spriteFile = this.FilePath.ToLower().Replace("cardimport", "png");

            if (File.Exists(spriteFile))
            {
                Sprite mySprite = await ImportHelper.GetSpriteAsync(spriteFile, 64, 64, mainThreadContext).ConfigureAwait(false);
                SpriteLookup.SetSprite(this.Id, mySprite);
            }
        }

        public override void ProcessAdditionalFiles()
        {
            string spriteFile = this.FilePath.ToLower().Replace("cardimport", "png");

            if (File.Exists(spriteFile))
            {
                Sprite mySprite = ImportHelper.GetSprite(spriteFile, 64, 64);
                SpriteLookup.SetSprite(this.Id, mySprite);
            }
        }
    }
}