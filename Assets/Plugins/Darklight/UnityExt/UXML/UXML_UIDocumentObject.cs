using System.Collections.Generic;
using Darklight.UnityExt.Utility;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.UXML
{
    /// <summary>
    /// A MonoBehaviour that handles the initialization of a UIDocument and manages its elements.
    /// It is suggested that you create a new ScriptableObject that inherits from UXML_UIDocumentPreset
    /// and assign it to the UIDocumentObject in the inspector.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class UXML_UIDocumentObject : MonoBehaviour
    {
        // << PUBLIC ACCESSORS >> //
        [SerializeField] public UXML_UIDocumentPreset preset;
        public UIDocument document => GetComponent<UIDocument>();
        public VisualElement root => document.rootVisualElement;
        public bool isVisible { get; protected set; }

        public virtual void Initialize(UXML_UIDocumentPreset preset, string[] tags = null)
        {
            this.preset = preset;
            if (preset == null)
            {
                Debug.LogError("No preset assigned to UIDocumentObject");
                return;
            }

            document.visualTreeAsset = preset.visualTreeAsset;

            // Create a new PanelSettings instance
            PanelSettings clonedPanelSettings = ScriptableObject.CreateInstance<PanelSettings>();

            // Copy properties from the original PanelSettings to the new one
            CopyPanelSettings(preset.panelSettings, clonedPanelSettings);
            document.panelSettings = clonedPanelSettings;

            // Assign the layer
            gameObject.layer = LayerMask.NameToLayer("UI");

            //Debug.Log($"Initialized UIDocumentObject with preset {preset.name}");
        }

        /// <summary>
        /// Query the root element for a VisualElement of the given type with an optional tag or class.
        /// </summary>
        /// <typeparam name="T">The type of VisualElement to query for.</typeparam>
        /// <param name="tagOrClass">Optional tag or class name to further refine the query.</param>
        /// <returns>The first matching element, or null if no match is found.</returns>
        public T ElementQuery<T>(string tagOrClass = null) where T : VisualElement
        {
            UQueryBuilder<T> query = root.Query<T>(tagOrClass);
            return query.First();
        }

        /// <summary>
        /// Query the root element for all VisualElements of the given type with an optional tag or class.
        /// </summary>
        /// <typeparam name="T">The type of VisualElement to query for.</typeparam>
        /// <param name="tagOrClass">Optional tag or class name to further refine the query.</param>
        /// <returns>An enumerable of all matching elements.</returns>
        public IEnumerable<T> ElementQueryAll<T>(string tagOrClass = null) where T : VisualElement
        {
            HashSet<T> elements = new HashSet<T>();
            root.Query<T>(tagOrClass).ForEach(element => elements.Add(element));
            return elements;
        }

        /// <summary>
        /// Toggle the visibility of the root element.
        /// </summary>
        public void ToggleVisibility()
        {
            isVisible = !isVisible;
            root.visible = isVisible;
        }

        /// <summary>
        /// Set the visibility of the root element directly.
        /// </summary>
        public void SetVisibility(bool visible)
        {
            isVisible = visible;
            root.visible = isVisible;
        }

        private void CopyPanelSettings(PanelSettings source, PanelSettings destination)
        {
            // Copy relevant properties from source to destination
            destination.scaleMode = source.scaleMode;
            destination.referenceResolution = source.referenceResolution;
            destination.screenMatchMode = source.screenMatchMode;
            destination.match = source.match;
            destination.sortingOrder = source.sortingOrder;
            destination.targetTexture = source.targetTexture;
            destination.clearColor = source.clearColor;
            destination.clearDepthStencil = source.clearDepthStencil;
            destination.sortingOrder = source.sortingOrder;
            destination.targetDisplay = source.targetDisplay;
            destination.targetDisplay = source.targetDisplay;
            destination.targetTexture = source.targetTexture;
            destination.clearColor = source.clearColor;
            destination.clearDepthStencil = source.clearDepthStencil;

            // Copy other necessary properties depending on your use case
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UXML_UIDocumentObject), true)]
    public class UXML_UIDocumentObjectCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        UXML_UIDocumentObject _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (UXML_UIDocumentObject)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (GUILayout.Button("Initialize"))
            {
                _script.Initialize(_script.preset);
            }

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

}