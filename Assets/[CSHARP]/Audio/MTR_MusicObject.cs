using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


[CreateAssetMenu(menuName = "MeetTheRookie/MTR_MusicObject")]
public class MTR_MusicObject : FMODExt_MusicObject
{
    //[Space(20), Header("---->> MEET THE ROOKIE SPECIFIC MUSIC CONTROLS")]

    private string musicIntensity = "MusicIntensity"; // Based on what it's called in FMOD

    /// <summary>
    /// Building off of the class inside "FMOD_MusicObject", this is a class to store a scene, event reference, and music intensity parameter for background music.
    /// </summary>
    [System.Serializable]
    public new class MTR_BackgroundMusicEvent : BackgroundMusicEvent
    { 
        [Range(0f, 2f)] // Based on the set parameter range in FMOD
        public int musicIntensityValue;
    }


    // -----------------------------------------------------------------------------------------
    public List<MTR_BackgroundMusicEvent> MTR_BackgroundMusicEvents = new List<MTR_BackgroundMusicEvent>();
    public new EventReference GetBackgroundMusicByScene(string sceneName)
    {
        MTR_BackgroundMusicEvent backgroundMusicEvent = MTR_BackgroundMusicEvents.Find(x => x.GetSceneName() == sceneName);
        if (backgroundMusicEvent == null)
        {
            Debug.LogError($"No Background Music Event found for scene: {sceneName}");
        }
        return backgroundMusicEvent.eventReference;
    }

    public void SetMusicIntensity(string sceneName)
    {
        MTR_BackgroundMusicEvent backgroundMusicEvent = MTR_BackgroundMusicEvents.Find(x => x.GetSceneName() == sceneName);
        if (backgroundMusicEvent == null)
        {
            Debug.LogError($"No Background Music Intensity found for scene: {sceneName}");
        }
        else
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(musicIntensity, (float)backgroundMusicEvent.musicIntensityValue);
        }
    }

}
