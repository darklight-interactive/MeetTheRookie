using Darklight.UnityExt.FMODExt;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MTR_AudioManager : FMODExt_EventManager
{
    // Overwrite the Instance property to return the instance of this class
    public static new MTR_AudioManager Instance => FMODExt_EventManager.Instance as MTR_AudioManager;

    // Overwrite the generalSFX property to return the instance of the MTR_GeneralSFX class
    private MTR_GeneralSFX sfx => GeneralSFX as MTR_GeneralSFX;
    private MTR_SceneManager sceneManager => MTR_SceneManager.Instance as MTR_SceneManager;

    public void StartFootstepEvent()
    {
        StartRepeatingEvent(sfx.footstep, sfx.footstepInterval);
    }
}
