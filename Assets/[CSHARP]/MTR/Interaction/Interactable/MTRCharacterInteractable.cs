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
    bool _speakerTagIsSet = false;
    List<string> _speakerList = new List<string>(30) { "None" };
    [SerializeField, ShowOnly] string _speakerTag = "None";
    [SerializeField, Dropdown("_speakerOptions"), HideIf("_speakerTagIsSet")] string _speakerTagDropdown;

    // ======== [[ PROPERTIES ]] ================================== >>>>
    List<string> _speakerOptions
    {
        get
        {
            if (MTRStoryManager.Instance.SpeakerList != null && MTRStoryManager.Instance.SpeakerList.Count > 4)
            {
                _speakerList = new List<string>() { "None" };
                _speakerList.AddRange(MTRStoryManager.Instance.SpeakerList);
            }
            return _speakerList;
        }
    }
    public override Type TypeKey => Type.CHARACTER;
    public string SpeakerTag => _speakerTag;

    void OnValidate()
    {
        if (_speakerTagDropdown != "" && _speakerTagDropdown != "None")
        {
            _speakerTag = _speakerTagDropdown;
            _speakerTagIsSet = true;
        }
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
        if (Data.Key == null && SpeakerTag != "None")
        {
            Data.SetKey(SpeakerTag);
        }
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