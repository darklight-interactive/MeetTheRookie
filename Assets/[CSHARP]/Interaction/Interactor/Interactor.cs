using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IInteractor
{
    LayerMask LayerMask { get; }
    Library<string, Interactable> NearbyInteractables { get; }
    Interactable TargetInteractable { get; }

    void TryAddInteractable(Interactable interactable);
    void RemoveInteractable(Interactable interactable);

    List<Interactable> FindInteractables();
    Interactable GetInteractable(string name);
    Interactable GetClosestReadyInteractable(Vector3 position);

    bool TryAssignTarget(Interactable interactable);
    void ClearTarget();

    bool InteractWith(Interactable interactable, bool force = false);
    bool InteractWithTarget();

    void RefreshNearbyInteractables();
}

[ExecuteAlways]
public class Interactor : MonoBehaviour, IInteractor
{
    protected const string PREFIX = "Interactor:";


    [Header("Interactor Settings")]
    [SerializeField] LayerMask _layerMask;
    [SerializeField] Vector2 _dimensions = new Vector2(1, 1);
    [SerializeField, ShowOnly] Vector2 _offsetPosition = new Vector2(0, 0);

    [Header("Interactables")]
    [SerializeField, ShowOnly] Interactable _lastTarget;
    [SerializeField, ShowOnly] Interactable _target;
    [SerializeField] Library<string, Interactable> _nearbyInteractables = new Library<string, Interactable>();

    // ======== [[ PROPERTIES ]] ================================== >>>>
    public LayerMask LayerMask { get => _layerMask; set => _layerMask = value; }
    public Library<string, Interactable> NearbyInteractables => _nearbyInteractables;
    public Interactable TargetInteractable => _target;

    public Vector2 OffsetPosition { get => _offsetPosition; set => _offsetPosition = value; }
    protected Vector2 OverlapCenter => (Vector2)transform.position + _offsetPosition;

    #region ======== <METHODS> (( UNITY RUNTIME )) ================================== >>>>
    public virtual void Update()
    {
        RefreshNearbyInteractables();

        // << UPDATE TARGET >> --------
        Interactable closestInteractable = GetClosestReadyInteractable(OverlapCenter);
        TryAssignTarget(closestInteractable);
    }

    void OnDrawGizmos()
    {
        CustomGizmos.DrawWireRect(OverlapCenter, _dimensions, Vector3.forward, Color.red);
        foreach (Interactable interactable in _nearbyInteractables.Values)
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

            Gizmos.DrawSphere(interactable.transform.position, 0.025f);
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
        if (!_nearbyInteractables.ContainsKey(interactable.name))
            _nearbyInteractables.Add(interactable.name, interactable);
    }

    public void RemoveInteractable(Interactable interactable)
    {
        if (_nearbyInteractables.ContainsKey(interactable.name))
            _nearbyInteractables.Remove(interactable.name);
    }

    public Interactable GetInteractable(string name)
    {
        // Retrieve the interactable by name if it exists.
        if (_nearbyInteractables.TryGetValue(name, out var interactable))
        {
            return interactable;
        }
        return null;
    }

    public Interactable GetClosestReadyInteractable(Vector3 position)
    {
        if (_nearbyInteractables.Count == 0) return null;
        if (_nearbyInteractables.Count == 1) return _nearbyInteractables.Values.First();

        Interactable closestInteractable = _nearbyInteractables.Values.First();
        float closestDistance = float.MaxValue;
        foreach (Interactable interactable in _nearbyInteractables.Values)
        {
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

        // Remove interactables from the dict that are no longer in the overlap interactables.
        List<Interactable> dictInteractables = new List<Interactable>(_nearbyInteractables.Values);
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
}

#if UNITY_EDITOR
[CustomEditor(typeof(Interactor), true)]
public class InteractorCustomEditor : UnityEditor.Editor
{
    SerializedObject _serializedObject;
    Interactor _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (Interactor)target;
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