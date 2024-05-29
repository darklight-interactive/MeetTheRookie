using UnityEngine;
using Darklight.UnityExt.Input;
using Darklight.Utility;
using Darklight.UnityExt;
using Darklight.UnityExt.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor;



[RequireComponent(typeof(UniversalInputManager))]
public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public static UniversalInputManager InputManager => UniversalInputManager.Instance;
    public static BuildSceneManager<MTR_SceneData> MTR_SceneManager => BuildSceneManager<MTR_SceneData>.Instance;
    public static InkyStoryManager StoryManager => InkyStoryManager.Instance;
    public static GameStateMachine StateMachine = new GameStateMachine(GameState.NULL);

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        Cursor.visible = false;

        MTR_SceneManager.OnSceneChange += OnSceneChanged;
    }

    public void OnSceneChanged(MTR_SceneData oldScene, MTR_SceneData newScene)
    {
        InputManager.Reset();
        InputManager.Awake();

        if (newScene.name == "MAIN_MENU")
        {
            StoryManager.Initialize();
        }
        
        //string sceneKnot = StoryManager.GetSceneKnot(newScene.name);
        //StoryManager.Iterator.GoToKnotOrStitch(sceneKnot);

        //Debug.Log($"{Prefix} Scene changed to {newScene.name}, Knot: {sceneKnot}");
    }


    public Vector3 GetMidpoint(Vector3 point1, Vector3 point2)
    {
        return (point1 + point2) * 0.5f;
    }
}

// ================================================================================================= //
// ------------ [[ GameStateMachine ]] ------------ //
public enum GameState { NULL, MAIN_MENU, LOADING_SCENE }
public class GameStateMachine : StateMachine<GameState>
{
    public GameStateMachine(GameState baseState) : base(baseState) { }
    public override void GoToState(GameState newState)
    {
        base.GoToState(newState);
    }

    public override void OnStateChanged(GameState previousState, GameState newState)
    {
        base.OnStateChanged(previousState, newState);
    }
}
