namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.UX;
    using SpaceDeck.GameState.Execution;

    public class PlayerChoiceController : MonoBehaviour
    {
        [SerializeReference]
        private CardBrowser CardBrowserUX;

        void OnEnable()
        {
            //  TODO REGISTER LISTENER GlobalUpdateUX.PlayerMustMakeChoice.AddListener(_HandlePlayerChoice);
        }

        void OnDisable()
        {
            //  TODO REGISTER LISTENER GlobalUpdateUX.PlayerMustMakeChoice.RemoveListener(_HandlePlayerChoice);
        }

        void HandlePlayerChoice(IGameStateMutator mutator, PlayerChoice toHandle, Action continuationAction)
        {
            if (toHandle is PlayerChooseFromCardBrowser cardBrowser)
            {
                this.CardBrowserUX.SetFromCardBrowserChoice(mutator, cardBrowser,
                    (List<CardInstance> chosenCards) =>
                    {
                        cardBrowser.SetChoice(mutator, chosenCards);
                        continuationAction?.Invoke();
                    });
                return;
            }

            // TODO LOG
        }
    }
}