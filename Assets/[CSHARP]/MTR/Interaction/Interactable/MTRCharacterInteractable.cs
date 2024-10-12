using UnityEngine;
using Darklight.UnityExt.Editor;
using System.Collections.Generic;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Utility;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class MTRCharacterInteractable : MTRInteractable
{
    [SerializeField, Dropdown("_speakerOptions")] string _speakerTag;

    // ======== [[ PROPERTIES ]] ================================== >>>>
    List<string> _speakerOptions
    {
        get
        {
            List<string> speakerList = new List<string>(50);
            if (MTRStoryManager.Instance.SpeakerList != null)
            {
                speakerList = MTRStoryManager.Instance.SpeakerList;
            }
            return speakerList;
        }
    }
    public override Type TypeKey => Type.CHARACTER;
    public string SpeakerTag => _speakerTag;


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

    protected override void GenerateRecievers()
    {
        InteractionSystem.Factory.CreateOrLoadInteractionRequest(TypeKey.ToString(),
            out InteractionRequestDataObject interactionRequest,
            new List<InteractionType> { InteractionType.TARGET, InteractionType.DIALOGUE });
        Request = interactionRequest;
        InteractionSystem.Factory.GenerateInteractableRecievers(this);
    }

    public override void Initialize()
    {
        base.Initialize();
        //Recievers.SetRequiredKeys(Data.InteractionRequest.Keys);
    }

    public void PlayAnimation(NPCState state)
    {
        GetComponent<NPC_Controller>().stateMachine.GoToState(NPCState.PLAY_ANIMATION);
    }
}