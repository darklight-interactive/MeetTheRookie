using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt;
using Darklight;
using Darklight.UnityExt.Input;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum GameState { NULL, MAIN_MENU, LOADING_SCENE }
public class GameManager : MonoBehaviour, IGameSingleton<GameManager>
{
    public static GameManager Instance = IGameSingleton<GameManager>.Instance;
    public static UXML_InteractionUI InteractionUI => ISceneSingleton<UXML_InteractionUI>.Instance;
    public static UniversalInputManager InputManager => UniversalInputManager.Instance;
    public static GameStateMachine StateMachine = new GameStateMachine(GameState.NULL);
    public static InkyKnotThreader InkyKnotThreader = ISingleton<InkyKnotThreader>.Instance;

    void Awake()
    {
        (this as IGameSingleton<GameManager>).Initialize();
        InkyKnotThreader.LoadStory("1.1_MelOMart");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    InkyKnotThreader threader;

    public override void OnInspectorGUI()
    {
        GameManager gameManager = (GameManager)target;
        threader = GameManager.InkyKnotThreader;

        GameState gameState = GameManager.StateMachine.CurrentState;
        CustomInspectorGUI.DrawEnumProperty(ref gameState, $"Game State");

        InkyKnotThreader.State threadState = GameManager.InkyKnotThreader.currentState;
        CustomInspectorGUI.DrawEnumProperty(ref threadState, $"InkyKnotThreader State");
        InkyKnotThreader.Console.DrawInEditor();

        CustomInspectorGUI.DrawDefaultInspectorWithoutSelfReference(this.serializedObject);

    }
}

#endif