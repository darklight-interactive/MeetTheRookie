using UnityEngine;

public class SceneChange_Interactable : Interactable
{
    [SerializeField] SceneObject _sceneTarget;

    public override void Interact()
    {
        base.Interact();
        SceneManager.Instance.LoadScene(_sceneTarget);
    }
}