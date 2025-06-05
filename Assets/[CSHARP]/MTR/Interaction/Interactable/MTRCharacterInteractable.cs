using System.Collections.Generic;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MTRCharacterInteractable : MTRInteractable
{
    bool _speakerTagIsSet = false;

    [SerializeField]
    MTRSpeaker _speakerTag = MTRSpeaker.UNKNOWN;

    // ======== [[ PROPERTIES ]] ================================== >>>>
    public override Type TypeKey => Type.CHARACTER;
    public MTRSpeaker SpeakerTag => _speakerTag;

    void OnValidate()
    {
        if (_speakerTag != MTRSpeaker.UNKNOWN)
            _speakerTagIsSet = true;
    }

    // ======== [[ METHODS ]] ================================== >>>>
    protected override void PreloadBoxCollider()
    {
        collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.size = new Vector2(0.5f, 1);
    }

    protected override void PreloadData()
    {
        base.PreloadData();
        if (Data.Key == null && SpeakerTag != MTRSpeaker.UNKNOWN)
        {
            Data.SetKey(SpeakerTag.ToString());
        }
    }

    protected override void GenerateRecievers()
    {
        InteractionSystem.Factory.CreateOrLoadInteractionRequest(
            TypeKey.ToString(),
            out InteractionRequestDataObject interactionRequest,
            new List<InteractionType> { InteractionType.TARGET, InteractionType.DIALOGUE }
        );
        Request = interactionRequest;
        InteractionSystem.Factory.GenerateInteractableRecievers(this);
    }

    public void PlayAnimation(NPCState state)
    {
        GetComponent<NPC_Controller>().stateMachine.GoToState(NPCState.PLAY_ANIMATION);
    }
}
