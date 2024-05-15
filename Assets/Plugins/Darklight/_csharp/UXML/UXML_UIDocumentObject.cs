using System.Collections.Generic;
using Darklight.Game.Utility;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UXML
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
        [ShowOnly] public UXML_UIDocumentPreset preset;
        public UIDocument document => GetComponent<UIDocument>();
        public VisualElement root => document.rootVisualElement;
        public Dictionary<string, VisualElement> uiElements { get; private set; } = new();
        public virtual void Initialize(UXML_UIDocumentPreset preset, string[] tags = null)
        {
            this.preset = preset;
            if (preset == null)
            {
                Debug.LogError("No preset assigned to UIDocumentObject");
                return;
            }

            document.visualTreeAsset = preset.VisualTreeAsset;
            document.panelSettings = preset.PanelSettings;
            gameObject.layer = LayerMask.NameToLayer("UI");
        }

        /// <summary>
        /// Query the root element for a VisualElement of the given type with an optional tag or class.
        /// </summary>
        /// <typeparam name="T">The type of VisualElement to query for.</typeparam>
        /// <param name="tagOrClass">Optional tag or class name to further refine the query.</param>
        /// <returns>The first matching element, or null if no match is found.</returns>
        public T ElementQuery<T>(string tagOrClass = null) where T : VisualElement
        {
            var query = root.Query<T>(tagOrClass);
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
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UXML_UIDocumentObject), true)]
    public class UXML_UIDocumentObjectCustomEditor : Editor
    {
        SerializedObject _serializedObject;
        UXML_UIDocumentObject _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (UXML_UIDocumentObject)target;
            _script.Initialize(_script.preset);
        }
    }
#endif

}