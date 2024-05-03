using System.Collections.Generic;
using Darklight.Game.Utility;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.UXML
{
    public interface I_UIDocument
    {
        UXML_UIDocumentPreset preset { get; }
        UIDocument document { get; }
        VisualElement root { get; }
        Dictionary<string, UXML_ControlledVisualElement> uiElements { get; }
    }

    /// <summary>
    /// A MonoBehaviour that handles the initialization of a UIDocument and manages its elements.
    /// It is suggested that you create a new ScriptableObject that inherits from UXML_UIDocumentPreset
    /// and assign it to the UIDocumentObject in the inspector.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class UXML_UIDocumentObject : MonoBehaviour, I_UIDocument
    {
        // << SERIALIZED VALUES >> //
        [SerializeField] UXML_UIDocumentPreset _preset;

        // << PUBLIC ACCESSORS >> //
        public UXML_UIDocumentPreset preset => _preset;
        public UIDocument document => GetComponent<UIDocument>();
        public VisualElement root => document.rootVisualElement;

        public Dictionary<string, UXML_ControlledVisualElement> uiElements { get; private set; } = new Dictionary<string, UXML_ControlledVisualElement>();

        public virtual void Initialize(UXML_UIDocumentPreset preset, string[] tags = null)
        {
            _preset = preset;
            if (preset == null)
            {
                Debug.LogError("No preset assigned to UIDocumentObject");
                return;
            }
            document.visualTreeAsset = preset.VisualTreeAsset;
            document.panelSettings = preset.PanelSettings;

            if (tags != null)
                LoadUIElements(tags);

            gameObject.layer = LayerMask.NameToLayer("UI");
        }

        /// <summary>
        /// Load the UI elements from the UIDocument with the given tags.
        /// </summary>
        /// <param name="tags"></param>
        void LoadUIElements(string[] tags)
        {
            foreach (string tag in tags)
            {
                VisualElement element = ElementQuery(tag);
                if (element != null)
                {
                    uiElements.Add(tag, new UXML_ControlledVisualElement(element, tag));
                }
                else
                {
                    Debug.LogWarning($"Element with tag {tag} not found in UIDocument {document.name}");
                }

                Debug.Log($"Element with tag {tag} found in UIDocument {document.name}");
            }
        }

        /// <summary>
        /// Query the root element for a VisualElement with the given tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        VisualElement ElementQuery(string tag)
        {
            VisualElement element = root.Query(tag);
            return element;
        }

        // ====== [[ PUBLIC METHODS ]] ================================
        public UXML_ControlledVisualElement GetUIElement(string tag)
        {
            // Get the element if it already exists
            if (uiElements.ContainsKey(tag))
            {
                return uiElements[tag];
            }

            // Create a new element if it exists in the UIDocument
            if (ElementQuery(tag) != null)
            {
                UXML_ControlledVisualElement newElement = new UXML_ControlledVisualElement(ElementQuery(tag), tag);
                uiElements.Add(tag, newElement);
                return newElement;
            }
            return null;
        }
    }
}