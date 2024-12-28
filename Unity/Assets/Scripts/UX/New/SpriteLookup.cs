namespace SpaceDeck.UX
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Utility.Wellknown;
    using Utility.Minimum;

    public static class SpriteLookup
    {
        public readonly static LowercaseString Default = nameof(Default);

        private static readonly Dictionary<LowercaseString, ObjectSpriteLookup> ObjectToSpriteLookup = new Dictionary<LowercaseString, ObjectSpriteLookup>();

        public static bool TryGetSprite(LowercaseString forObject, out Sprite forSprite)
        {
            return TryGetSprite(forObject, out forSprite, Default);
        }

        public static bool TryGetSprite(LowercaseString forObject, out Sprite forSprite, LowercaseString index)
        {
            if (!ObjectToSpriteLookup.TryGetValue(forObject, out ObjectSpriteLookup lookupTable))
            {
                forSprite = null;
                return false;
            }

            return lookupTable.TryGetSprite(index, out forSprite);
        }

        public static void SetSprite(LowercaseString forObject, Sprite forSprite)
        {
            SetSprite(forObject, forSprite, Default);
        }

        public static void SetSprite(LowercaseString forObject, Sprite forSprite, LowercaseString index)
        {
            if (ObjectToSpriteLookup.TryGetValue(forObject, out ObjectSpriteLookup lookup))
            {
                lookup.SetSprite(index, forSprite);
            }
            else
            {
                ObjectToSpriteLookup.Add(forObject, new ObjectSpriteLookup());
                SetSprite(forObject, forSprite, index);
            }
        }

        public static void Clear()
        {
            ObjectToSpriteLookup.Clear();
        }
    }

    public class ObjectSpriteLookup
    {
        private readonly Dictionary<LowercaseString, Sprite> IndexToSprites = new Dictionary<LowercaseString, Sprite>();

        public bool TryGetSprite(LowercaseString index, out Sprite sprite)
        {
            return this.IndexToSprites.TryGetValue(index, out sprite);
        }

        public void SetSprite(LowercaseString index, Sprite sprite)
        {
            this.IndexToSprites.Add(index, sprite);
        }
    }
}
