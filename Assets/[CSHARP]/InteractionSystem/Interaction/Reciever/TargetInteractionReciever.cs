using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.UXML;
using UnityEngine;
using NaughtyAttributes;
using Darklight.UnityExt.Editor;
using System.Collections;



#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Grid2D_OverlapWeightSpawner))]
public class TargetInteractionReciever : InteractionReciever
{
    [SerializeField, Expandable] UXML_UIDocumentPreset _interactIconPreset;
    [SerializeField, ShowOnly] UXML_RenderTextureObject _interactIconObject;
    [SerializeField, ShowOnly] bool _visible = false;

    Grid2D_OverlapWeightSpawner gridSpawner => GetComponent<Grid2D_OverlapWeightSpawner>();
    Material material => MTR_UIManager.Instance.UXML_RenderTextureMaterial;
    RenderTexture renderTexture => MTR_UIManager.Instance.UXML_RenderTexture;

    public override InteractionType InteractionType => InteractionType.TARGET;

    public void Update()
    {

    }

    public void ShowInteractIcon()
    {
        // Skip if in editor mode
        if (Application.isPlaying == false) return;
        Debug.Log($"{gameObject.name}: ShowInteractIcon called.", this);

        // << Get the best cell available >>
        Cell2D cell = gridSpawner.GetBestCell();
        cell.GetTransformData(out Vector3 position, out Vector2 dimensions, out Vector3 normal);

        if (cell != null)
        {
            if (cell.Position == Vector3.zero && cell.Dimensions == Vector2.one)
            {
                Debug.LogError($"Cell {cell.Name} has default values, calling ShowInteractIcon() again.");
                Invoke(nameof(ShowInteractIcon), 0.5f);
                return;
            }
        }

        // << Create a new interact icon >>
        if (_interactIconObject == null)
        {
            // Create the UXML RenderTextureObject Icon
            _interactIconObject = UXML_Utility.CreateUXMLRenderTextureObject(_interactIconPreset, material, renderTexture);

            // Set the Icon as a child of the InteractIconSpawner
            _interactIconObject.transform.SetParent(transform);
        }

        _interactIconObject.transform.position = position + (normal * 0.1f);
        _interactIconObject.SetLocalScale(dimensions.y);
        _interactIconObject.SetVisibility(true);
        _interactIconObject.TextureUpdate();
    }

    public void HideInteractIcon()
    {
        // Log the start of the method with the GameObject name as a prefix
        Debug.Log($"{gameObject.name}: HideInteractIcon called.", this);

        // Check if _interactIconObject is null and no children with UXML_RenderTextureObject exist
        if (_interactIconObject == null && GetComponentInChildren<UXML_RenderTextureObject>() == null)
        {
            Debug.LogWarning($"{gameObject.name}: _interactIconObject is null and no UXML_RenderTextureObject found in children. Exiting method.", this);
            return;
        }

        // Attempt to get the _interactIconObject if it is null
        _interactIconObject = GetComponentInChildren<UXML_RenderTextureObject>();
        if (_interactIconObject == null)
        {
            Debug.LogError($"{gameObject.name}: Failed to find UXML_RenderTextureObject in children after checking. Exiting method.", this);
            return;
        }

        // Log the destruction of the GameObject
        Debug.Log($"{gameObject.name}: Destroying Interact Icon GameObject: {_interactIconObject.gameObject.name}", this);
        ObjectUtility.DestroyAlways(_interactIconObject.gameObject);

        // Resetting _interactIconObject to null and log it
        _interactIconObject = null;
        Debug.Log($"{gameObject.name}: _interactIconObject has been set to null.", this);

        // Set visibility flag to false and log it
        _visible = false;
        Debug.Log($"{gameObject.name}: _visible flag set to false.", this);

        // Log the end of the method
        Debug.Log($"{gameObject.name}: HideInteractIcon completed successfully.", this);
    }

}