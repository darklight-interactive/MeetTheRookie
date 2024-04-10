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

    void Awake()
    {
        (this as IGameSingleton<GameManager>).Initialize();

        InkyStoryThreader threader = InkyStoryThreader.Instance;
        threader.StartThread();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameManager gameManager = (GameManager)target;

        GameState gameState = GameManager.StateMachine.CurrentState;
        CustomInspectorGUI.DrawEnumProperty(ref gameState, $"GameStateMachine");

        DrawDefaultInspector();

    }
}

#endif