using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Inky;
using System.Collections;
using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using Darklight.UnityExt.Library;
using System.Linq;



#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class MTRPlayerInteractor : MTRCharacterInteractable, IInteractor
{
    const float INTERACTOR_X_OFFSET = 0.35f;

    [Header("Interactor Settings")]
    [SerializeField] LayerMask _layerMask;
    [SerializeField] Vector2 _dimensions = new Vector2(1, 1);
    [SerializeField, ShowOnly] Vector2 _offsetPosition = new Vector2(0, 0);

    [Header("Interactables")]
    [SerializeField, ShowOnly] Interactable _lastTarget;
    [SerializeField, ShowOnly] Interactable _target;

    [Space(10)]
    [SerializeField, ShowOnly] Interactable _closestInteractable;

    [SerializeField]
    protected Library<Interactable, string> _nearbyInteractables
        = new Library<Interactable, string>()
        {
            ReadOnlyKey = true,
            ReadOnlyValue = true
        };

    // ======== [[ PROPERTIES ]] ================================== >>>>
    public override Type TypeKey => Type.PLAYER_INTERACTOR;
    public LayerMask LayerMask { get => _layerMask; set => _layerMask = value; }
    public Library<Interactable, string> NearbyInteractables => _nearbyInteractables;
    public Interactable TargetInteractable => _target;

    public Vector2 OffsetPosition { get => _offsetPosition; set => _offsetPosition = value; }
    protected Vector2 OverlapCenter => (Vector2)transform.position + _offsetPosition;



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
    #endregion

    // ======== [[ METHODS ]] ================================== >>>>

    #region ======== <METHODS> (( UNITY RUNTIME )) ================================== >>>>
    void OnDrawGizmos()
    {
        CustomGizmos.DrawWireRect(OverlapCenter, _dimensions, Vector3.forward, Color.red);
        foreach (Interactable interactable in _nearbyInteractables.Keys)
        {
            if (interactable == null) continue;
            if (interactable == _target)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, interactable.transform.position);
            }
            else
            {
                Gizmos.color = Color.yellow;
            }

            Gizmos.DrawSphere(interactable.transform.position, 0.05f);
        }
    }
    #endregion

    #region ======== <METHODS> (( MTRCharacterInteractable )) ================================== >>>>
    protected override void PreloadData()
    {
        base.PreloadData();
        Data.SetName("Lupe");

        // Set the layer mask to the interactor
        LayerMask = InteractionSystem.Settings.GetCombinedNPCAndInteractableLayer();
    }

    protected override void GenerateRecievers()
    {
        InteractionSystem.Factory.CreateOrLoadInteractionRequest(TypeKey.ToString(),
            out InteractionRequestDataObject interactionRequest,
            new List<InteractionType> { InteractionType.DIALOGUE });
        Request = interactionRequest;
        InteractionSystem.Factory.GenerateInteractableRecievers(this);
    }

    public override void Refresh()
    {
        base.Refresh();

        RefreshNearbyInteractables();

        // << UPDATE TARGET >> --------
        _closestInteractable = GetClosestReadyInteractable(OverlapCenter);
        TryAssignTarget(_closestInteractable);

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

    #endregion

    #region ======== <PUBLIC_METHODS> (( IInteractor )) ================================== >>>>
    public List<Interactable> FindInteractables()
    {
        List<Interactable> interactables = new List<Interactable>();
        Collider2D[] colliders = Physics2D.OverlapBoxAll(OverlapCenter, _dimensions, 0, _layerMask);
        foreach (Collider2D collider in colliders)
        {
            Interactable interactable = collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactables.Add(interactable);
            }
        }
        return interactables;
    }

    public void TryAddInteractable(Interactable interactable)
    {
        if (interactable == null) return;
        if (!_nearbyInteractables.ContainsKey(interactable))
            _nearbyInteractables.Add(interactable, interactable.Name);
        else
            _nearbyInteractables[interactable] = interactable.Name;
    }

    public void RemoveInteractable(Interactable interactable)
    {
        if (interactable == null) return;
        if (_nearbyInteractables.ContainsKey(interactable))
            _nearbyInteractables.Remove(interactable);
    }

    public Interactable GetClosestReadyInteractable(Vector3 position)
    {
        if (_nearbyInteractables.Count == 0) return null;
        if (_nearbyInteractables.Count == 1) return _nearbyInteractables.Keys.First();

        Interactable closestInteractable = _nearbyInteractables.Keys.First();
        float closestDistance = float.MaxValue;
        foreach (Interactable interactable in _nearbyInteractables.Keys)
        {
            if (interactable == null) continue;

            // Calculate the distance to the interactable.
            float distance = Vector3.Distance(interactable.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }
        return closestInteractable;
    }

    public bool TryAssignTarget(Interactable interactable)
    {
        if (interactable == null) return false;
        if (_target == interactable) return false;
        if (_lastTarget == interactable) return false;

        bool result = interactable.AcceptTarget(this);
        if (result)
        {
            _lastTarget = _target;
            _target = interactable;

            if (_lastTarget != null)
                _lastTarget.Reset();
        }
        //Debug.Log($"[{name}] TryAssignTarget: {interactable.name} => {result}");
        return result;
    }

    public void ClearTarget()
    {
        _lastTarget = _target;
        _target = null;

        _lastTarget.Reset();
    }

    public bool InteractWith(Interactable interactable, bool force = false)
    {
        if (interactable == null) return false;
        return interactable.AcceptInteraction(this, force);
    }

    public bool InteractWithTarget() => InteractWith(_target);

    public void RefreshNearbyInteractables()
    {
        // Update the interactables dictionary with the overlap interactables.
        List<Interactable> overlapInteractables = FindInteractables();
        foreach (Interactable interactable in overlapInteractables)
        {
            TryAddInteractable(interactable);
        }

        if (_target != null && !overlapInteractables.Contains(_target))
        {
            _target.Reset();
            _target = null;
        }

        if (_lastTarget != null && !overlapInteractables.Contains(_lastTarget))
        {
            _lastTarget.Reset();
            _lastTarget = null;
        }

        // Remove interactables from the dict that are no longer in the overlap interactables.
        List<Interactable> dictInteractables = new List<Interactable>(_nearbyInteractables.Keys);
        List<Interactable> interactablesToRemove = new List<Interactable>();
        foreach (Interactable interactable in dictInteractables)
        {
            if (!overlapInteractables.Contains(interactable))
            {
                interactablesToRemove.Add(interactable);
            }
        }
        foreach (Interactable interactable in interactablesToRemove)
        {
            RemoveInteractable(interactable);
        }
    }

    #endregion

    /*
    public override void Preload()
    {
        if (Data == null)
            data = new InteractableData(this, "Lupe", "Player");
        else
            BaseInternalData.Preload(this, "Lupe", "Player");

        // Set the layer mask to the interactor
        LayerMask = InteractionSystem.Settings.GetCombinedNPCAndInteractableLayer();
    }
    */

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

}
