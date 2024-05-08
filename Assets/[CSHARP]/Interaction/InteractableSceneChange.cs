using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UXML;
using UnityEngine.UIElements;
using Darklight.Game.Grid;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class InteractableSceneChange : Interactable, IInteract
{
    [Header("Scene Change Settings")]
    [SerializeField] private SceneAsset sceneAsset;

    public void Start()
    {
        this.OnCompleted += () =>
        {
            if (sceneAsset)
            {
                SceneManager.LoadScene(sceneAsset.name);
            }
        };
    }

}
