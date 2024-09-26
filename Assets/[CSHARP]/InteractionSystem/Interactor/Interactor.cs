using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;
using System.Linq;
using NaughtyAttributes;


#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IInteractor
{
    LayerMask LayerMask { get; }
    public Library<BaseInteractable, string> NearbyInteractables { get; }
    BaseInteractable TargetInteractable { get; }

    void TryAddInteractable(BaseInteractable interactable);
    void RemoveInteractable(BaseInteractable interactable);

    List<BaseInteractable> FindInteractables();
    BaseInteractable GetClosestReadyInteractable(Vector3 position);

    bool TryAssignTarget(BaseInteractable interactable);
    void ClearTarget();

    bool InteractWith(BaseInteractable interactable, bool force = false);
    bool InteractWithTarget();

    void RefreshNearbyInteractables();
}

[ExecuteAlways]
public class Interactor : BaseInteractable, IInteractor
{

    [Header("Interactor Settings")]
    [SerializeField] LayerMask _layerMask;
    [SerializeField] Vector2 _dimensions = new Vector2(1, 1);
    [SerializeField, ShowOnly] Vector2 _offsetPosition = new Vector2(0, 0);

    [Header("Interactables")]
    [SerializeField, ShowOnly] BaseInteractable _lastTarget;
    [SerializeField, ShowOnly] BaseInteractable _target;

    [Space(10)]
    [SerializeField, ShowOnly] BaseInteractable _closestInteractable;

    [SerializeField]
    protected Library<BaseInteractable, string> _nearbyInteractables
        = new Library<BaseInteractable, string>()
        {
            ReadOnlyKey = true,
            ReadOnlyValue = true
        };

    // ======== [[ PROPERTIES ]] ================================== >>>>
    public LayerMask LayerMask { get => _layerMask; set => _layerMask = value; }
    public Library<BaseInteractable, string> NearbyInteractables => _nearbyInteractables;
    public BaseInteractable TargetInteractable => _target;

    public Vector2 OffsetPosition { get => _offsetPosition; set => _offsetPosition = value; }
    protected Vector2 OverlapCenter => (Vector2)transform.position + _offsetPosition;

    #region ======== <METHODS> (( UNITY RUNTIME )) ================================== >>>>
    public virtual void Update()
    {
        RefreshNearbyInteractables();

        // << UPDATE TARGET >> --------
        _closestInteractable = GetClosestReadyInteractable(OverlapCenter);
        TryAssignTarget(_closestInteractable);
    }

    void OnDrawGizmos()
    {
        CustomGizmos.DrawWireRect(OverlapCenter, _dimensions, Vector3.forward, Color.red);
        foreach (BaseInteractable interactable in _nearbyInteractables.Keys)
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

    #region ======== <PUBLIC_METHODS> (( IInteractor )) ================================== >>>>
    public List<BaseInteractable> FindInteractables()
    {
        List<BaseInteractable> interactables = new List<BaseInteractable>();
        Collider2D[] colliders = Physics2D.OverlapBoxAll(OverlapCenter, _dimensions, 0, _layerMask);
        foreach (Collider2D collider in colliders)
        {
            BaseInteractable interactable = collider.GetComponent<BaseInteractable>();
            if (interactable != null)
            {
                interactables.Add(interactable);
            }
        }
        return interactables;
    }

    public void TryAddInteractable(BaseInteractable interactable)
    {
        if (interactable == null) return;
        if (!_nearbyInteractables.ContainsKey(interactable))
            _nearbyInteractables.Add(interactable, interactable.Name);
        else
            _nearbyInteractables[interactable] = interactable.Name;
    }

    public void RemoveInteractable(BaseInteractable interactable)
    {
        if (interactable == null) return;
        if (_nearbyInteractables.ContainsKey(interactable))
            _nearbyInteractables.Remove(interactable);
    }

    public BaseInteractable GetClosestReadyInteractable(Vector3 position)
    {
        if (_nearbyInteractables.Count == 0) return null;
        if (_nearbyInteractables.Count == 1) return _nearbyInteractables.Keys.First();

        BaseInteractable closestInteractable = _nearbyInteractables.Keys.First();
        float closestDistance = float.MaxValue;
        foreach (BaseInteractable interactable in _nearbyInteractables.Keys)
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

    public bool TryAssignTarget(BaseInteractable interactable)
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

    public bool InteractWith(BaseInteractable interactable, bool force = false)
    {
        if (interactable == null) return false;
        return interactable.AcceptInteraction(this, force);
    }

    public bool InteractWithTarget() => InteractWith(_target);

    public void RefreshNearbyInteractables()
    {
        // Update the interactables dictionary with the overlap interactables.
        List<BaseInteractable> overlapInteractables = FindInteractables();
        foreach (BaseInteractable interactable in overlapInteractables)
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
        List<BaseInteractable> dictInteractables = new List<BaseInteractable>(_nearbyInteractables.Keys);
        List<BaseInteractable> interactablesToRemove = new List<BaseInteractable>();
        foreach (BaseInteractable interactable in dictInteractables)
        {
            if (!overlapInteractables.Contains(interactable))
            {
                interactablesToRemove.Add(interactable);
            }
        }
        foreach (BaseInteractable interactable in interactablesToRemove)
        {
            RemoveInteractable(interactable);
        }
    }

    #endregion
}