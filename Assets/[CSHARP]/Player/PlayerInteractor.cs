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

public class PlayerInteractor : Interactor
{
    DialogueInteractionHandler _dialogueHandler;
    ChoiceInteractionHandler _choiceHandler;

    [SerializeField, Dropdown("_speakerOptions")] string _speakerTag;

    [Header("Interactable Radar")]
    [ShowOnly, Tooltip("The Interactable that the player is currently targeting.")]
    public Interactable targetInteractable;

    [ShowOnly, Tooltip("The Interactable that the player is currently interacting with. Can be null.")]
    public Interactable activeInteractable;
    [ShowOnly, Tooltip("The Interactable that the player was previously targeting. Can be null.")]
    public Interactable previousTargetInteractable;

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

    public MTR_InteractionManager InteractionManager => MTR_InteractionManager.Instance;
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
        MTR_InteractionManager.RegisterPlayerInteractor(this);
    }

    public bool InteractWithTarget()
    {
        if (targetInteractable == null) return false;
        if (targetInteractable.isComplete) return false;

        targetInteractable.TargetClear();
        previousTargetInteractable = null;
        // If first interaction
        if (activeInteractable != targetInteractable)
        {
            // Set the active interactable
            activeInteractable = targetInteractable;

            activeInteractable.OnCompleted += ExitInteraction;

            StartCoroutine(MoveToPosition());

            return true;
        }

        activeInteractable.Interact(); // << MAIN INTERACTION
        return true;
    }

    private IEnumerator MoveToPosition()
    {
        PlayerController controller = gameObject.GetComponent<PlayerController>();

        // Set up destination points and sort by nearest
        List<GameObject> destinationPoints = activeInteractable.GetDestinationPoints();

        // Ensure there are enough destination points
        if (destinationPoints == null || destinationPoints.Count == 0)
        {
            //Debug.LogError("No destination points found.");
            // Set the player controller state to Interaction
            PlayerController.EnterInteraction();

            // Set Lupe to face interactable
            Vector3 newActiveInteractablePosition = activeInteractable.gameObject.transform.position;
            PlayerController.animator.FrameAnimationPlayer.FlipTransform(new Vector2(newActiveInteractablePosition.x < gameObject.transform.position.x ? -1 : 1, 0));

            activeInteractable.Interact(); // << MAIN INTERACTION
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
        controller.destinationPoint.destinationPoint = nearestDestinationPoint.GetComponent<DestinationPoint>();
        controller.stateMachine.GoToState(PlayerState.WALKOVERRIDE);

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

        // Set the player controller state to Interaction
        PlayerController.EnterInteraction();

        if (Misra != null)
        {
            Misra.GetComponent<MTR_Misra_Controller>().stateMachine.GoToState(NPCState.FOLLOW);
        }

            // Set Lupe to face interactable
            Vector3 activeInteractablePosition = activeInteractable.gameObject.transform.position;
        PlayerController.animator.FrameAnimationPlayer.FlipTransform(new Vector2(activeInteractablePosition.x < gameObject.transform.position.x ? -1 : 1, 0));

        activeInteractable.Interact(); // << MAIN INTERACTION
    }

    public void ForceInteract(Interactable interactable)
    {
        if (interactable == null) return;
        Debug.Log($"Player Interactor :: Force Interact with {interactable.name}");

        // Set the target interactable
        targetInteractable = interactable;
        targetInteractable.TargetSet();

        // Interact with the target
        InteractWithTarget();
    }

    public void ExitInteraction()
    {
        Debug.Log("Player Interactor :: Exit Interaction");

        // Clean up
        //MTR_UIManager.Instance.DestroySpeechBubble();
        PlayerController.ExitInteraction();

        // Force set the speaker to Lupe
        InkyStoryManager.Instance.SetSpeaker("Lupe");

        // Unsubscribe from the OnComplete event
        activeInteractable.OnCompleted -= ExitInteraction;
        targetInteractable = null;
        activeInteractable = null;
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
