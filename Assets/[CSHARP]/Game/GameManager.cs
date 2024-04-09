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
    public GameStateMachine gameStateMachine = new GameStateMachine(GameState.NULL);
    UXML_InteractionUI uXML_InteractionUI => ISceneSingleton<UXML_InteractionUI>.Instance;
    UniversalInputManager universalInputManager => UniversalInputManager.Instance;
    void Awake()
    {
        (this as IGameSingleton<GameManager>).Initialize();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private void OnEnable()
    {

    }

    public override void OnInspectorGUI()
    {
        GameManager gameManager = (GameManager)target;

        GameState gameState = gameManager.gameStateMachine.CurrentState;
        CustomInspectorGUI.DrawEnumProperty(ref gameState, $"GameStateMachine");

        DrawDefaultInspector();

    }
}

#endif