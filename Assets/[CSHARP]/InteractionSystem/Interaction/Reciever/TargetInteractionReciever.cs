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
        //Debug.Log($"{gameObject.name}: ShowInteractIcon called.", this);

        // << Get the best cell available >>
        Cell2D bestCell = gridSpawner.GetBestCell();
        bestCell.GetTransformData(out Vector3 position, out Vector2 dimensions, out Vector3 normal);

        if (bestCell != null)
        {
            if (bestCell.Position == Vector3.zero && bestCell.Dimensions == Vector2.one)
            {
                Debug.LogError($"Cell {bestCell.Name} has default values, calling ShowInteractIcon() again.");
                Invoke(nameof(ShowInteractIcon), 0.5f);
                return;
            }
        }

        // << Create a new interact icon >>
        if (_interactIconObject == null)
        {
            // Create the UXML RenderTextureObject Icon
            _interactIconObject = UXML_Utility.CreateUXMLRenderTextureObject(_interactIconPreset, material, renderTexture);
            //_interactIconObject.document.panelSettings = _interactIconPreset.panelSettings;

            Cell2D.SpawnerComponent spawnerComponent = bestCell.GetComponent<Cell2D.SpawnerComponent>();
            spawnerComponent.AttachTransformToCell(_interactIconObject.transform);

            // Set the Icon as a child of the InteractIconSpawner
            _interactIconObject.transform.SetParent(transform);
        }

        _interactIconObject.SetLocalScale(dimensions.y);
        _visible = true;
    }

    public void HideInteractIcon()
    {
        // Log the start of the method with the GameObject name as a prefix
        //Debug.Log($"{gameObject.name}: HideInteractIcon called.", this);

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

        ObjectUtility.DestroyAlways(_interactIconObject.gameObject);
        _interactIconObject = null;
        _visible = false;
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(TargetInteractionReciever))]
    public class TargetInteractionRecieverCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        TargetInteractionReciever _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (TargetInteractionReciever)target;
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (_script._visible)
            {
                if (GUILayout.Button("Hide Interact Icon"))
                {
                    _script.HideInteractIcon();
                }
            }

            if (!_script._visible)
            {
                if (GUILayout.Button("Show Interact Icon"))
                {
                    _script.ShowInteractIcon();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}