using System.Collections.Generic;
using FMODUnity;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Darklight/FMOD/FMODEventObject")]
public class FMODBackgroundMusicObject : ScriptableObject
{

    [System.Serializable]
    public class BackgroundMusicEvent
    {
        public SceneAsset scene;
        public EventReference eventReference;

        public string GetSceneName()
        {
            return scene.name;
        }
    }

    [Header("Background Music")]
    public List<BackgroundMusicEvent> backgroundMusicEvents = new List<BackgroundMusicEvent>();

    public EventReference GetEventReference(string sceneName)
    {
        BackgroundMusicEvent backgroundMusicEvent = backgroundMusicEvents.Find(x => x.scene.name == sceneName);
        if (backgroundMusicEvent == null)
        {
            Debug.LogError($"No Background Music Event found for scene: {sceneName}");
        }
        return backgroundMusicEvent.eventReference;
    }
}