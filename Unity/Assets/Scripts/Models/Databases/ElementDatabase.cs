namespace SpaceDeck.Models.Databases
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections.Generic;

    public static class ElementDatabase
    {
        public static Dictionary<LowercaseString, Element> ElementData { get; set; } = new Dictionary<LowercaseString, Element>();

        public static void AddElement(ElementImport toAdd)
        {
            Element newElement = new Element(toAdd.Id, toAdd.Name);
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

            // TODO: LOG FAILURE

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