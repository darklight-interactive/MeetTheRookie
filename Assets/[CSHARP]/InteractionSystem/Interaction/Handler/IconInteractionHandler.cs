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

public class IconInteractionHandler : Grid2D_OverlapWeightSpawner, IInteractionHandler
{
    [SerializeField, Expandable] UXML_UIDocumentPreset _interactIconPreset;
    [SerializeField, ShowOnly] UXML_RenderTextureObject _interactIconObject;
    [SerializeField, ShowOnly] bool _visible = false;

    Material material => MTR_UIManager.Instance.UXML_RenderTextureMaterial;
    RenderTexture renderTexture => MTR_UIManager.Instance.UXML_RenderTexture;

    public InteractionTypeKey TypeKey => InteractionTypeKey.ICON;
    public bool IsVisible => _visible;

    public void ShowInteractIcon()
    {
        // << Get the best cell available >>
        Cell2D cell = GetBestCell();
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

        _visible = true;
    }

    public void HideInteractIcon()
    {
        if (_interactIconObject == null) return;

        DestroyInteractIcon();

        /*
        _interactIconObject.SetVisibility(false);
        _interactIconObject.TextureUpdate();

        _visible = false;
        */
    }

    public void DestroyInteractIcon()
    {
        if (_interactIconObject == null) return;

        ObjectUtility.DestroyAlways(_interactIconObject.gameObject);
        _interactIconObject = null;

        _visible = false;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(IconInteractionHandler))]
    public class InteractIconSpawnerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        IconInteractionHandler _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (IconInteractionHandler)target;
            _script.Awake();
        }

        void OnDisable()
        {
            _script.DestroyInteractIcon();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            DrawButtons();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }

        void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (!_script.IsVisible || _script._interactIconObject == null)
            {
                if (GUILayout.Button("Show Interact Icon"))
                {
                    _script.ShowInteractIcon();
                }
            }
            else if (_script._interactIconObject != null)
            {
                if (_script.IsVisible && GUILayout.Button("Hide Interact Icon"))
                {
                    _script.HideInteractIcon();
                }

                if (GUILayout.Button("Destroy Interact Icon"))
                {
                    _script.DestroyInteractIcon();
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}