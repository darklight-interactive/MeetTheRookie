using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Utility;
using UnityEngine;

public class SceneManager : MonoBehaviourSingleton<SceneManager>
{

    [SerializeField] private UXML_UIDocumentPreset sceneTransitionPreset;

    // >> SCENE OBJECTS <<
    // These 
    [SerializeField] private SceneObject MainMenu;
    [SerializeField] private SceneObject Scene1_1;
    [SerializeField] private SceneObject Scene1_2;

    public void LoadScene(SceneObject sceneObject)
    {
        UXML_SceneTransition sceneTransition = new GameObject("SceneTransition").AddComponent<UXML_SceneTransition>();
        sceneTransition.Initialize(sceneTransitionPreset, new string[] { "blackborder", "textlabel" });
        sceneTransition.BeginFadeOut(sceneObject);
    }
}
