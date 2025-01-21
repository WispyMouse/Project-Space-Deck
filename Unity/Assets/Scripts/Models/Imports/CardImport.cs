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
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.UX.AssetLookup;
    using UnityEngine;

    [System.Serializable]
    public class CardImport : Importable
    {
        public string Name;
        public string EffectScript;
        public List<ElementGainImport> ElementGain;

        public CardPrototype GetPrototype()
        {
            if (!TokenTextMaker.TryGetTokenTextFromString(this.EffectScript, out TokenText statement))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.CardImport, $"Failed to determine token text from string. '{this.EffectScript}'");
                return null;
            }
            else if (!ParsedTokenMaker.TryGetParsedTokensFromTokenText(statement, out ParsedTokenList parsedTokens))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.CardImport, $"Failed to parse tokens from token text. '{this.EffectScript}'");
                return null;
            }
            else
            {
                Dictionary<LowercaseString, int> elementGain = new Dictionary<LowercaseString, int>();
                foreach (ElementGainImport import in (this.ElementGain != null ? (IEnumerable<ElementGainImport>)this.ElementGain : Array.Empty<ElementGainImport>()))
                {
                    elementGain.Add(import.ElementId, import.ModAmount);
                }
                return new CardPrototype(this.Id, parsedTokens, elementGain);
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