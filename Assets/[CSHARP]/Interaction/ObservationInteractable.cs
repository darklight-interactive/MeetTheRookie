using UnityEngine;

public class ObservationInteractable : InkyInteractable
{

    public FMODUnity.EventReference interactionSound;

    public override void OnDestroy()
    {
        //throw new System.NotImplementedException();
    }

    protected override void Initialize()
    {
        //throw new System.NotImplementedException();
    }

    public override void Interact()
    {
        FMODManager.Instance.PlayEvent(interactionSound);
        base.Interact();
    }
}