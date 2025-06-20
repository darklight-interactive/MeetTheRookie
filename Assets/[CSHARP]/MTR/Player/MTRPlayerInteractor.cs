using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(MTRPlayerController))]
public class MTRPlayerInteractor : MTRCharacterInteractable, IInteractor
{
    const float INTERACTOR_X_OFFSET = 0.35f;

    [Header("Interactor Settings")]
    [SerializeField, ShowOnly]
    bool _enabled = false;

    [SerializeField]
    LayerMask _layerMask;

    [SerializeField]
    Vector2 _dimensions = new Vector2(1, 1);

    [SerializeField, ShowOnly]
    Vector2 _offsetPosition = new Vector2(0, 0);

    [Header("Interactables")]
    [SerializeField, ShowOnly]
    Interactable _lastTarget;

    [SerializeField, ShowOnly]
    Interactable _target;

    [Space(10)]
    [SerializeField, ShowOnly]
    Interactable _closestInteractable;

    [SerializeField]
    protected Library<Interactable, string> _nearbyInteractables = new Library<
        Interactable,
        string
    >()
    {
        ReadOnlyKey = true,
        ReadOnlyValue = true
    };

    // ======== [[ PROPERTIES ]] ================================== >>>>
    public MTRPlayerController Controller => GetComponent<MTRPlayerController>();
    public override Type TypeKey => Type.PLAYER;
    public LayerMask LayerMask
    {
        get => _layerMask;
        set => _layerMask = value;
    }
    public Library<Interactable, string> NearbyInteractables => _nearbyInteractables;
    public Interactable TargetInteractable => _target;

    public Vector2 OffsetPosition
    {
        get => _offsetPosition;
        set => _offsetPosition = value;
    }
    protected Vector2 OverlapCenter => (Vector2)transform.position + _offsetPosition;

    // ======== [[ EVENTS ]] ================================== >>>>
    public event Action<Interactable> OnInteractableAccepted;

    #region ======== [[ PROPERTIES ]] ================================== >>>>
    public MTRPlayerController PlayerController => GetComponent<MTRPlayerController>();
    #endregion

    // ======== [[ METHODS ]] ================================== >>>>

    #region ======== <METHODS> (( UNITY RUNTIME )) ================================== >>>>
    void OnDrawGizmosSelected()
    {
        if (!_enabled)
            return;

        CustomGizmos.DrawWireRect(OverlapCenter, _dimensions, Vector3.forward, Color.red);
        foreach (Interactable interactable in _nearbyInteractables.Keys)
        {
            if (interactable == null)
                continue;
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

    public override void Refresh()
    {
        if (!_enabled)
            return;

        base.Refresh();
        RefreshNearbyInteractables();

        // << UPDATE TARGET >> --------
        _closestInteractable = GetClosestReadyInteractable(OverlapCenter);
        TryAssignTarget(_closestInteractable);

        // << UPDATE FACING >> --------
        if (PlayerController.DirectionFacing == MTRPlayerDirectionFacing.LEFT)
        {
            OffsetPosition = new Vector2(-INTERACTOR_X_OFFSET, 0);

            // If the target is to the right of the player, clear the target
            if (
                TargetInteractable != null
                && TargetInteractable.transform.position.x > transform.position.x
            )
                ClearTarget();
        }
        else if (PlayerController.DirectionFacing == MTRPlayerDirectionFacing.RIGHT)
        {
            OffsetPosition = new Vector2(INTERACTOR_X_OFFSET, 0);

            // If the target is to the left of the player, clear the target
            if (
                TargetInteractable != null
                && TargetInteractable.transform.position.x < transform.position.x
            )
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
        if (interactable == null)
            return;
        if (!_nearbyInteractables.ContainsKey(interactable))
            _nearbyInteractables.Add(interactable, interactable.Name);
        else
            _nearbyInteractables[interactable] = interactable.Name;
    }

    public void RemoveInteractable(Interactable interactable)
    {
        if (interactable == null)
            return;
        if (_nearbyInteractables.ContainsKey(interactable))
            _nearbyInteractables.Remove(interactable);
    }

    public Interactable GetClosestReadyInteractable(Vector3 position)
    {
        if (_nearbyInteractables.Count == 0)
            return null;
        if (_nearbyInteractables.Count == 1)
            return _nearbyInteractables.Keys.First();

        Interactable closestInteractable = _nearbyInteractables.Keys.First();
        float closestDistance = float.MaxValue;
        foreach (Interactable interactable in _nearbyInteractables.Keys)
        {
            if (interactable == null)
                continue;

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
        if (!_enabled)
            return false;

        if (interactable == null)
            return false;
        if (_target == interactable)
            return false;
        if (_lastTarget == interactable)
            return false;

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
        if (!_enabled)
            return;

        _lastTarget = _target;
        _target = null;
        _lastTarget.Reset();
    }

    public bool InteractWith(Interactable interactable, bool force = false)
    {
        if (interactable == null)
            return false;

        _target = interactable;

        Debug.Log($"[{name}] Interacting with: {interactable.name} : force={force}");

        bool result = interactable.AcceptInteraction(this, force);
        if (result)
            OnInteractableAccepted?.Invoke(interactable);
        return result;
    }

    public bool InteractWithTarget() => InteractWith(_target);

    public void RefreshNearbyInteractables()
    {
        if (!_enabled)
            return;

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

    public void SetEnabled(bool enabled)
    {
        if (_enabled == enabled)
            return;

        _enabled = enabled;
        Debug.Log($"[{name}] Interactor Enabled: {_enabled}");
    }
}
