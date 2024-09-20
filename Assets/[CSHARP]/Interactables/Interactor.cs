using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;



#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IInteractor
{
    LayerMask LayerMask { get; }
    List<Interactable> Interactables { get; }
    List<Interactable> GetOverlapInteractables();
    void TryAddInteractable(Interactable interactable);
    void RemoveInteractable(Interactable interactable);
    Interactable GetInteractable(string name);
    Interactable GetClosestInteractableTo(Vector3 position);
    void Refresh();
}

[ExecuteAlways]
public class Interactor : MonoBehaviour, IInteractor
{
    const string PREFIX = "Interactor:";
    [SerializeField] Library<string, Interactable> _interactableLibrary = new Library<string, Interactable>();
    [SerializeField] LayerMask _layerMask;
    [SerializeField, Range(0.1f, 100f)] float _scale = 1f;

    public List<Interactable> Interactables => new List<Interactable>(_interactableLibrary.Values);
    public LayerMask LayerMask => _layerMask;

    void Update()
    {
        Refresh();
    }

    public void OnDrawGizmosSelected()
    {
        CustomGizmos.DrawWireSquare(transform.position, _scale, Vector3.forward, Color.green);

        foreach (Interactable interactable in _interactableLibrary.Values)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(interactable.transform.position, 0.1f);
        }
    }

    #region ======== <PUBLIC_METHODS> (( IInteractor )) ================================== >>>>
    public List<Interactable> GetOverlapInteractables()
    {
        List<Interactable> interactables = new List<Interactable>();
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, Vector2.one * _scale, 0, LayerMask);
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
        if (!_interactableLibrary.ContainsKey(interactable.name))
            _interactableLibrary.Add(interactable.name, interactable);
    }

    public void RemoveInteractable(Interactable interactable)
    {
        if (_interactableLibrary.ContainsKey(interactable.name))
            _interactableLibrary.Remove(interactable.name);
    }

    public Interactable GetInteractable(string name)
    {
        // Retrieve the interactable by name if it exists.
        if (_interactableLibrary.TryGetValue(name, out var interactable))
        {
            return interactable;
        }
        return null;
    }

    public Interactable GetClosestInteractableTo(Vector3 position)
    {
        Interactable closestInteractable = null;
        float closestDistance = float.MaxValue;
        foreach (Interactable interactable in _interactableLibrary.Values)
        {
            float distance = Vector3.Distance(interactable.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }
        return closestInteractable;
    }

    public void Refresh()
    {
        // Update the interactables dictionary with the overlap interactables.
        List<Interactable> overlapInteractables = GetOverlapInteractables();
        foreach (Interactable interactable in overlapInteractables)
        {
            TryAddInteractable(interactable);
        }

        // Remove interactables from the dict that are no longer in the overlap interactables.
        List<Interactable> dictInteractables = new List<Interactable>(_interactableLibrary.Values);
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