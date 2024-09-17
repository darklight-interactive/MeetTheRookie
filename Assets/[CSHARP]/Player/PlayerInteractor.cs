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

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] DialogueInteractionHandler _dialogueHandler;
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
    [SerializeField, Dropdown("_speakerOptions")] string _speakerTag;
    [SerializeField, ShowOnly] protected List<Interactable> _foundInteractables = new List<Interactable>();

    [ShowOnly, Tooltip("The Interactable that the player is currently targeting.")]
    public Interactable targetInteractable;

    [ShowOnly, Tooltip("The Interactable that the player is currently interacting with. Can be null.")]
    public Interactable activeInteractable;
    [ShowOnly, Tooltip("The Interactable that the player was previously targeting. Can be null.")]
    public Interactable previousTargetInteractable;

    // ======== [[ PROPERTIES ]] ================================== >>>>
    public PlayerController PlayerController => GetComponent<PlayerController>();
    public DialogueInteractionHandler DialogueHandler => _dialogueHandler;
    public string SpeakerTag => _speakerTag;

    // ======== [[ METHODS ]] ================================== >>>>
    public void Awake()
    {
        if (_dialogueHandler == null)
        {
            _dialogueHandler = GetComponentInChildren<DialogueInteractionHandler>();
            if (_dialogueHandler == null)
            {
                _dialogueHandler = ObjectUtility.InstantiatePrefabWithComponent<DialogueInteractionHandler>
                    (MTR_UIManager.Instance.dialogueSpawnerPrefab, Vector3.zero, Quaternion.identity, transform);
                _dialogueHandler.transform.localPosition = Vector3.zero;
            }
        }
        _dialogueHandler.SpeakerTag = _speakerTag;
    }


    #region -- [[ UPDATE THE INTERACTABLE RADAR ]] ------------------------------------- >> 

    public void Update()
    {
        RefreshRadar();
    }

    void RefreshRadar()
    {
        if (_foundInteractables.Count == 0) return;
        if (PlayerController.currentState == PlayerState.INTERACTION) return;
        /*
        // Temporary list to hold items to be removed
        List<Interactable> toRemove = new List<Interactable>();

        // Iterate through the found interactables
        foreach (Interactable interactable in _foundInteractables)
        {
            if (interactable == null) continue;
            if (interactable.isComplete)
            {
                // Mark the interaction for removal
                toRemove.Add(interactable);
                interactable.TargetClear();
            }
        }

        // Remove the completed interactions from the HashSet
        foreach (Interactable completedInteraction in toRemove)
        {
            _foundInteractables.Remove(completedInteraction);
        }
        */
        // Update the target interactable
        targetInteractable = GetClosestInteractable();
        // Only set the target if the interactable is not the active target
        if (targetInteractable != activeInteractable && !targetInteractable.isComplete)
        {
            if(previousTargetInteractable == null || targetInteractable != previousTargetInteractable){
                targetInteractable.TargetSet();
                previousTargetInteractable = targetInteractable;
            }
        }
        else if (targetInteractable != null){
            targetInteractable.TargetClear();
            previousTargetInteractable = null;
        }
    }

    #endregion

    Interactable GetClosestInteractable()
    {
        if (_foundInteractables.Count == 0) return null;

        Interactable closest = null;
        float closestDistance = float.MaxValue;

        foreach (Interactable interactable in _foundInteractables)
        {
            float distance = Vector2.Distance(transform.position, interactable.transform.position);
            if (distance < closestDistance)
            {
                closest = interactable;
                closestDistance = distance;
            }
        }

        return closest;
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

    /// <summary>
    /// Remove interactables from the local list and clear their target state. 
    /// </summary>
    public void ClearInteractables()
    {
        foreach (Interactable interactable in _foundInteractables)
        {
            interactable.TargetClear();
        }
        _foundInteractables.Clear();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        Debug.Log($"Player Interactor :: {interactable}");

        if (interactable == null) return;
        if (interactable.isComplete) return;
        _foundInteractables.Add(interactable);

        // Set as target
        targetInteractable = interactable;
        interactable.TargetSet();
    }


    void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable == null) return;
        _foundInteractables.Remove(interactable);
        if(targetInteractable == interactable)
            interactable.TargetClear();
            previousTargetInteractable = null;
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
