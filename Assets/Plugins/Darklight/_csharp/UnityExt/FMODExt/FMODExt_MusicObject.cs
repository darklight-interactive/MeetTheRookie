using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using FMODUnity;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Darklight/FMODExt/MusicObject")]
public class FMODExt_MusicObject : ScriptableObject
{

    /// <summary>
    /// A generic class to store a scene and an event reference for background music.
    /// </summary>
    [System.Serializable]
    public class BackgroundMusicEvent
    {
        public SceneObject scene;
        public EventReference eventReference;

        public string GetSceneName()
        {
            return scene;
        }
    }

    // -----------------------------------------------------------------------------------------
    public List<BackgroundMusicEvent> backgroundMusicEvents = new List<BackgroundMusicEvent>();
    public EventReference GetBackgroundMusicByScene(string sceneName)
    {
        BackgroundMusicEvent backgroundMusicEvent = backgroundMusicEvents.Find(x => x.GetSceneName() == sceneName);
        if (backgroundMusicEvent == null)
        {
            Debug.LogError($"No Background Music Event found for scene: {sceneName}");
        }
        return backgroundMusicEvent.eventReference;
    }
}