namespace SpaceDeck.Tests.EditMode.UX
{
    using NUnit.Framework;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Events;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.Tokenization.ScriptingCommands;
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.GameState.Context;
    using SpaceDeck.Tokenization.Evaluatables.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;
    using System.Linq;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.UX.AssetLookup;

    public class SpriteLookupTests
    {
        [TearDown]
        public void TearDown()
        {
            SpriteLookup.Clear();
        }

        [Test]
        public void SpriteLookup_Fetches()
        {
            // ARRANGE
            LowercaseString id = "debug";
            Sprite newSprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1f, 1f), Vector2.zero);

            // ACT
            SpriteLookup.SetSprite(id, newSprite);

            // ASSERT
            Assert.IsTrue(SpriteLookup.TryGetSprite(id, out Sprite fetchSprite), "Should be able to get the sprite that was assigned.");
            Assert.AreEqual(newSprite, fetchSprite, "Should be fetching the identical same sprite.");
        }
    }
}