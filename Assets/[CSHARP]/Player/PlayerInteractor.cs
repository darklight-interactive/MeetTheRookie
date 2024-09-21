using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Inky;
using System.Collections;
using Darklight.UnityExt.Utility;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class PlayerInteractor : Interactor
{
    const float INTERACTOR_X_OFFSET = 0.35f;
    DialogueInteractionHandler _dialogueHandler;
    ChoiceInteractionHandler _choiceHandler;

    [HorizontalLine(color: EColor.Gray)]
    [SerializeField, Dropdown("_speakerOptions")] string _speakerTag;

    #region ======== [[ PROPERTIES ]] ================================== >>>>
    List<string> _speakerOptions
    {
        // This is just a getter a list of all the speakers in the story
        get
        {
            List<string> speakers = new List<string>();
            if (InkyStoryManager.Instance != null)
            {
                speakers = InkyStoryManager.SpeakerList;
            }
            return speakers;
        }
    }
    public PlayerController PlayerController => GetComponent<PlayerController>();
    public DialogueInteractionHandler DialogueHandler
    {
        get
        {
            if (_dialogueHandler == null)
                _dialogueHandler = GetComponentInChildren<DialogueInteractionHandler>();
            return _dialogueHandler;
        }
        set => _dialogueHandler = value;
    }
    public ChoiceInteractionHandler ChoiceHandler
    {
        get
        {
            if (_choiceHandler == null)
                _choiceHandler = GetComponentInChildren<ChoiceInteractionHandler>();
            return _choiceHandler;
        }
        set => _choiceHandler = value;
    }
    public string SpeakerTag => _speakerTag;
    #endregion

    // ======== [[ METHODS ]] ================================== >>>>
    public void Awake()
    {
        //MTR_InteractionManager.RegisterPlayerInteractor(this);
    }

    public override void Update()
    {
        base.Update();

        // << UPDATE FACING >> --------
        if (PlayerController.Facing == PlayerFacing.LEFT)
        {
            OffsetPosition = new Vector2(-INTERACTOR_X_OFFSET, 0);

            // If the target is to the right of the player, clear the target
            if (TargetInteractable != null &&
                TargetInteractable.transform.position.x > transform.position.x)
                ClearTarget();
        }
        else if (PlayerController.Facing == PlayerFacing.RIGHT)
        {
            OffsetPosition = new Vector2(INTERACTOR_X_OFFSET, 0);

            // If the target is to the left of the player, clear the target
            if (TargetInteractable != null &&
                TargetInteractable.transform.position.x < transform.position.x)
                ClearTarget();
        }
    }

    private IEnumerator MoveToPosition()
    {
        PlayerController controller = gameObject.GetComponent<PlayerController>();
        yield return new WaitForSeconds(0.1f);

        /*
        // Set up destination points and sort by nearest
        List<GameObject> destinationPoints = _activeInteractable.GetDestinationPoints();

        // Ensure there are enough destination points
        if (destinationPoints == null || destinationPoints.Count == 0)
        {
            //Debug.LogError("No destination points found.");
            // Set the player controller state to Interaction
            PlayerController.EnterInteraction();

            // Set Lupe to face interactable
            Vector3 newActiveInteractablePosition = _activeInteractable.gameObject.transform.position;
            //PlayerController.Animator.FrameAnimationPlayer.FlipSprite(new Vector2(newActiveInteractablePosition.x < gameObject.transform.position.x ? -1 : 1, 0));

            //_activeInteractable.AcceptInteraction(); // << MAIN INTERACTION
            yield break;
        }

        destinationPoints.Sort((a, b) =>
            Mathf.Abs(a.transform.position.x - PlayerController.transform.position.x)
                .CompareTo(Mathf.Abs(b.transform.position.x - PlayerController.transform.position.x))
        );
        GameObject nearestDestinationPoint = destinationPoints[0];
        GameObject secondNearest = null;

        // Find Misra
        GameObject Misra = null;
        var tempMisra = FindFirstObjectByType<MTR_Misra_Controller>();
        if (tempMisra != null)
        {
            Misra = tempMisra.gameObject;
        }

        // Make the DestinationPoints track Lupe and Misra
        nearestDestinationPoint.GetComponent<DestinationPoint>().trackedEntity = gameObject;
        if (destinationPoints.Count > 1 && Misra != null)
        {
            secondNearest = destinationPoints[1];
            secondNearest.GetComponent<DestinationPoint>().trackedEntity = Misra;
        }

        // Make Lupe and Misra walk to the points
        controller.DestinationPoint.destinationPoint = nearestDestinationPoint.GetComponent<DestinationPoint>();
        controller.StateMachine.GoToState(PlayerState.WALKOVERRIDE);

        if (Misra != null)
        {
            if (secondNearest == null)
            {
                Debug.LogError("Cannot move Misra, no destination point");
            } else
            {
                MTR_Misra_Controller Misra_Controller = Misra.GetComponent<MTR_Misra_Controller>();
                Misra_Controller.walkDestinationX = secondNearest.transform.position.x;
                Misra_Controller.stateMachine.GoToState(NPCState.WALK);
            }
        }

        // pause action until they reach the destinations
        while (true) {
            yield return new WaitForSeconds(0.05f);
            
            if (!nearestDestinationPoint.GetComponent<DestinationPoint>().isEntityInRange()) {
                continue;
            }

            if (secondNearest != null && Misra != null && !secondNearest.GetComponent<DestinationPoint>().isEntityInRange())
            {
                continue;
            }

            break;
        }

        if (Misra != null)
        {
            Misra.GetComponent<MTR_Misra_Controller>().stateMachine.GoToState(NPCState.FOLLOW);
        }

        */

        // Set the player controller state to Interaction
        PlayerController.EnterInteraction();



        // Set Lupe to face interactable
        //Vector3 activeInteractablePosition = _activeInteractable.gameObject.transform.position;
        //PlayerController.Animator.FrameAnimationPlayer.FlipSprite(new Vector2(activeInteractablePosition.x < gameObject.transform.position.x ? -1 : 1, 0));

        //_activeInteractable.AcceptInteraction(); // << MAIN INTERACTION
    }

    public void ExitInteraction()
    {
        Debug.Log("Player Interactor :: Exit Interaction");

        // Clean up
        //MTR_UIManager.Instance.DestroySpeechBubble();
        PlayerController.ExitInteraction();

        // Force set the speaker to Lupe
        InkyStoryManager.Instance.SetSpeaker("Lupe");
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerInteractor))]
    public class PlayerInteractorCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        PlayerInteractor _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (PlayerInteractor)target;
            _script.Awake();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}
