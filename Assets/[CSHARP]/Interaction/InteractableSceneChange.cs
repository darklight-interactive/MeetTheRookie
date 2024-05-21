using UnityEngine;
using UnityEngine.SceneManagement;
using Darklight.UnityExt.Editor;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class InteractableSceneChange : Interactable, IInteract
{
    [Header("Scene Change Settings")]
    [SerializeField] private SceneObject sceneObject;

    public void Start()
    {
        this.OnCompleted += () =>
        {
            if (SceneManager.GetActiveScene().name != sceneObject)
            {
                SceneManager.LoadScene(sceneObject);
            }
        };
    }
}
