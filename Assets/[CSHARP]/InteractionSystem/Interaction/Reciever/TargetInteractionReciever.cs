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

    public void ShowInteractIcon()
    {
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
        if (_interactIconObject == null) return;

        ObjectUtility.DestroyAlways(_interactIconObject.gameObject);
        _interactIconObject = null;

        _visible = false;
    }
}