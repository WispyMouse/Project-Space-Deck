using System.Collections;
using System.Collections.Generic;
using SpaceDeck.GameState.Minimum;
using SpaceDeck.UX;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenEditorPanel : MonoBehaviour
{
    public static bool ShowOpenEditorPanelOutsideOfNewLanding = false;

    // TODO: Can we make a reference for this?
    public string EditorScene = "EditorCenter";

    private void Awake()
    {
        this.MakeVisible();

        CentralGameStateController.OnResetGame += this.MakeVisible;
        CentralGameStateController.OnRouteChosen += (Route _) => this.ShowIfAppropriate();
    }

    public void OpenEditorPressed()
    {
        SceneManager.LoadScene(this.EditorScene);
    }

    public void MakeVisible()
    {
        this.gameObject.SetActive(true);
    }

    public void ShowIfAppropriate()
    {
        this.gameObject.SetActive(ShowOpenEditorPanelOutsideOfNewLanding);
    }
}
