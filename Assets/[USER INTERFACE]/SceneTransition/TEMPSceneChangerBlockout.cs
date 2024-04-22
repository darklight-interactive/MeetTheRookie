using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TEMPSceneChangerBlockout : MonoBehaviour
{
    public SceneManagerScript gm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            gm = FindFirstObjectByType<SceneManagerScript>();
            gm.newSceneName = "MAIN_MENU";
        }
    }
}
