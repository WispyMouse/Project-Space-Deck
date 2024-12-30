namespace SFDDCards
{
    using SFDDCards.ImportModels;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;

    [Obsolete("Should use " + nameof(SpaceDeck.Models.Databases.ElementDatabase))]
    public static class ElementDatabase
    {
        public static Dictionary<string, Element> ElementData { get; set; } = new Dictionary<string, Element>();

        public static void AddElement(ElementImport toAdd)
        {
            Element newElement = new Element()
            {
                Id = toAdd.Id.ToLower(),
                Name = toAdd.Name,
                Sprite = toAdd.NormalArt,
                GreyscaleSprite = toAdd.GreyscaleArt
            };

            ElementData.Add(toAdd.Id.ToLower(), newElement);
        }

        public static bool TryGetElement(string id, out Element foundElement)
        {
            return ElementData.TryGetValue(id, out foundElement);
        }

        public static Element GetElement(string id)
        {
            if (ElementData.TryGetValue(id.ToLower(), out Element foundElement))
            {
                return foundElement;
            }

            GlobalUpdateUX.LogTextEvent.Invoke($"Failed to parse element {id}.", GlobalUpdateUX.LogType.RuntimeError);

            return null;
        }

        public static void ClearDatabase()
        {
            ElementData.Clear();
        }

        public static int GetElementGain(Element forElement, CardInstance forCard)
        {
            return (int)forCard.Qualities.GetNumericQuality(WellknownElements.GetElementGain(forElement.Id));
        }
    }
}