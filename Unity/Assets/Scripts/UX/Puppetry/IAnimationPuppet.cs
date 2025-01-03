namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;

    public interface IAnimationPuppet
    {
        Transform OwnTransform { get; }
        bool IsNotDestroyed { get; }
    }
}