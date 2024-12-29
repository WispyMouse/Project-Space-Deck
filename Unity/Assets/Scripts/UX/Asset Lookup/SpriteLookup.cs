namespace SpaceDeck.UX.AssetLookup
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.GameState.Minimum;

    public static class SpriteLookup
    {
        private static readonly Dictionary<LowercaseString, ObjectLookup<Sprite>> ObjectToSpriteLookup = new Dictionary<LowercaseString, ObjectLookup<Sprite>>();
        private static readonly ObjectLookup<int> ObjectToSpriteIndexLookup = new ObjectLookup<int>();

        public static bool TryGetSprite(LowercaseString forObject, out Sprite forSprite)
        {
            return TryGetSprite(forObject, out forSprite, WellknownSprites.Default);
        }

        public static bool TryGetSprite(LowercaseString forObject, out Sprite forSprite, LowercaseString index)
        {
            if (!ObjectToSpriteLookup.TryGetValue(forObject, out ObjectLookup<Sprite> lookupTable))
            {
                forSprite = null;
                return false;
            }

            return lookupTable.TryGet(index, out forSprite);
        }

        public static bool TryGetSpriteIndex(LowercaseString forObject, out int spriteIndex)
        {
            return ObjectToSpriteIndexLookup.TryGet(forObject, out spriteIndex);
        }

        public static void SetSprite(LowercaseString forObject, Sprite forSprite)
        {
            SetSprite(forObject, forSprite, WellknownSprites.Default);
        }

        public static void SetSprite(LowercaseString forObject, Sprite forSprite, LowercaseString index)
        {
            if (ObjectToSpriteLookup.TryGetValue(forObject, out ObjectLookup<Sprite> lookup))
            {
                lookup.Set(index, forSprite);
            }
            else
            {
                ObjectToSpriteLookup.Add(forObject, new ObjectLookup<Sprite>());
                SetSprite(forObject, forSprite, index);
            }
        }

        public static void SetSpriteIndex(LowercaseString forObject, int toIndex)
        {
            ObjectToSpriteIndexLookup.Set(forObject, toIndex);
        }

        public static void Clear()
        {
            ObjectToSpriteLookup.Clear();
        }



        public static string GetNameOrIcon(Currency currency)
        {
            if (TryGetSpriteIndex(currency.Id, out int index))
            {
                return $"<sprite index={index}>";
            }

            return currency.Name;
        }

        public static string GetNameAndMaybeIcon(Currency currency)
        {
            if (TryGetSpriteIndex(currency.Id, out int index))
            {
                return $"<sprite index={index}>{currency.Name}";
            }

            return currency.Name;
        }
    }

    public class ObjectLookup<T>
    {
        private readonly Dictionary<LowercaseString, T> IndexToSprites = new Dictionary<LowercaseString, T>();

        public bool TryGet(LowercaseString index, out T sprite)
        {
            return this.IndexToSprites.TryGetValue(index, out sprite);
        }

        public void Set(LowercaseString index, T sprite)
        {
            this.IndexToSprites.Add(index, sprite);
        }
    }
}
