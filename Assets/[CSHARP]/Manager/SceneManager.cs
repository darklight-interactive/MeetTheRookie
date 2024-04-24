using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Utility;
using UnityEngine;

public class SceneManager : MonoBehaviourSingleton<SceneManager>
{

    [SerializeField] private UXML_UIDocumentPreset sceneTransitionPreset;

    public void ChangeSceneTo(string newSceneName)
    {
        UXML_SceneTransition sceneTransition = new GameObject("SceneTransition").AddComponent<UXML_SceneTransition>();
        sceneTransition.Initialize(sceneTransitionPreset, new string[] { "blackborder", "textlabel" });
        sceneTransition.BeginFadeOut(newSceneName);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangeSceneTo("SCENE1.1_MelOMart");
        }
    }
}
