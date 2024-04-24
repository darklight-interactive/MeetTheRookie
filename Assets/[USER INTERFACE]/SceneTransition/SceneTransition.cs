using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Utility;
using UnityEngine;

public class SceneTransition : MonoBehaviourSingleton<SceneTransition>
{
    public UXML_UIDocumentPreset SceneManager;
    public static SceneManagerScript SceneManagerScript;
    // Start is called before the first frame update
    void Start()
    {
        SceneManagerScript = new GameObject("SceneManager").AddComponent<SceneManagerScript>();
        SceneManagerScript.transform.SetParent(transform);
        
    }

}
