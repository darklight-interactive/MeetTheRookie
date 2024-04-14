using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt;
using Darklight;
using Darklight.UnityExt.Input;
using Darklight.Game;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum GameState { NULL, MAIN_MENU, LOADING_SCENE }
public class GameStateMachine : StateMachine<GameState>
{
    public GameStateMachine(GameState baseState) : base(baseState) { }
    public override void ChangeState(GameState newState)
    {
        base.ChangeState(newState);
    }

    public override void OnStateChanged(GameState previousState, GameState newState)
    {
        base.OnStateChanged(previousState, newState);
    }
}

public class GameManager : MonoBehaviour, IGameSingleton<GameManager>
{
    public static GameManager Instance = IGameSingleton<GameManager>.Instance;
    public static UXML_InteractionUI InteractionUI => ISceneSingleton<UXML_InteractionUI>.Instance;
    public static UniversalInputManager InputManager => UniversalInputManager.Instance;
    public static GameStateMachine StateMachine = new GameStateMachine(GameState.NULL);
    public static InkyKnotThreader InkyKnotThreader = ISingleton<InkyKnotThreader>.Instance;
    public string initialStoryPath = "scene1";

    void Awake()
    {
        (this as IGameSingleton<GameManager>).Initialize();
    }

    void Start()
    {
        InkyKnotThreader.LoadStory(initialStoryPath);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{

    bool showThreader = true;
    InkyKnotThreader threader;
    public override void OnInspectorGUI()
    {
        GameManager gameManager = (GameManager)target;
        threader = GameManager.InkyKnotThreader;

        GameState gameState = GameManager.StateMachine.CurrentState;
        CustomInspectorGUI.DrawEnumProperty(ref gameState, $"Game State");

        CustomInspectorGUI.CreateFoldout(ref showThreader, "InkyKnotThreader", ShowThreader);

        CustomInspectorGUI.DrawDefaultInspectorWithoutSelfReference(this.serializedObject);

    }

    void ShowThreader()
    {
        InkyKnotThreader.State threadState = GameManager.InkyKnotThreader.currentState;
        CustomInspectorGUI.DrawEnumProperty(ref threadState, $"InkyKnotThreader State");
        InkyKnotThreader.Console.DrawInEditor();
    }
}

#endif