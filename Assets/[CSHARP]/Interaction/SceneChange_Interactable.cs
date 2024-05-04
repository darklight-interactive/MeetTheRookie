using Darklight.UnityExt.Editor;
using UnityEngine;

public class SceneChange_Interactable : Interactable
{
    [SerializeField] SceneObject _sceneTarget;

    public override void Interact()
    {
        base.Interact();
        //SceneManager.Instance.LoadScene(_sceneTarget);
    }

    public override void OnDestroy()
    {
        //throw new System.NotImplementedException();
    }

    protected override void Initialize()
    {
        //throw new System.NotImplementedException();
    }
}