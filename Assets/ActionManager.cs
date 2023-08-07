using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public CharacterControl characterControl;
    public SceneManager sceneManager;
    /*
    reference to:
    - player,
    - scene manager,
    - quest manager,
    */

    /*
    receive:  {item}
    delete: {item}
    clear inventory: {}
    sendPlayerTo: {location}
    enterMinigame: {minigame}
    enterCutscene: {cutscene}
    */

    public static void recieve(string item){
        Debug.Log("Received" + item);
    }

}
