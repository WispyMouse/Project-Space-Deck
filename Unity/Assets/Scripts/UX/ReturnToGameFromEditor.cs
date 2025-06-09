using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToGameFromEditor : MonoBehaviour
{
    public string GameScene = "SampleScene";

    public void Clicked()
    {
        SceneManager.LoadScene(this.GameScene);
    }
}
