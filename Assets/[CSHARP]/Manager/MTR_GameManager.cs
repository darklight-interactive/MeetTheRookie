using Darklight.UnityExt.Audio;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Input;
using Darklight.UnityExt.Utility;
using Darklight.Utility;

using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UniversalInputManager))]
public class MTR_GameManager : MonoBehaviourSingleton<MTR_GameManager>
{
    public static UniversalInputManager InputManager => UniversalInputManager.Instance;
    public static MTR_SceneManager GameSceneManager => MTR_SceneManager.Instance as MTR_SceneManager; 
    public static InkyStoryManager StoryManager => InkyStoryManager.Instance;
    public static GameStateMachine StateMachine = new GameStateMachine(GameState.NULL);

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        Cursor.visible = false;

        GameSceneManager.OnSceneChanged += OnSceneChanged;
    }

    public void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        InputManager.Reset();
        InputManager.Awake();


        MTR_SceneData newSceneData = GameSceneManager.GetSceneData(newScene);
        StoryManager.Iterator.GoToKnotOrStitch(newSceneData.knot);

        FMODEventManager.Instance.PlaySong(newSceneData.backgroundMusicEvent);
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
