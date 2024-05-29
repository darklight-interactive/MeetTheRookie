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

    // This instance points to the abstract BuildSceneManager class, so we need to cast it to the custom class.
    public static MTR_SceneManager BuildSceneManager => MTR_SceneManager.Instance as MTR_SceneManager; 
    public static InkyStoryManager StoryManager => InkyStoryManager.Instance;
    public static GameStateMachine StateMachine = new GameStateMachine(GameState.NULL);

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        Cursor.visible = false;

        BuildSceneManager.OnSceneChange += OnSceneChanged;

        StoryManager.Initialize();
    }

    public void OnSceneChanged(MTR_SceneData oldSceneData, MTR_SceneData newSceneData)
    {
        InputManager.Reset();
        InputManager.Awake();

        if (newSceneData.name == "MAIN_MENU")
        {
            StoryManager.Initialize();
        }

        StoryManager.Iterator.GoToKnotOrStitch(newSceneData.knot);


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
