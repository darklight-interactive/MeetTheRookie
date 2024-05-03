using System.Collections.Generic;
using Darklight.Game.Utility;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.UXML
{
    interface I_UIDocument
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
        public UXML_UIDocumentPreset preset { get; private set; }

        public UIDocument document => GetComponent<UIDocument>();

        public VisualElement root => document.rootVisualElement;

        public Dictionary<string, UXML_ControlledVisualElement> uiElements { get; private set; } = new Dictionary<string, UXML_ControlledVisualElement>();

        public virtual void Initialize(UXML_UIDocumentPreset preset, string[] tags)
        {
            this.preset = preset;
            document.visualTreeAsset = preset.VisualTreeAsset;
            document.panelSettings = preset.PanelSettings;

            if (tags != null)
            {
                LoadUIElements(tags);
            }

            gameObject.layer = LayerMask.NameToLayer("UI");
        }

        void LoadUIElements(string[] tags)
        {
            foreach (string tag in tags)
            {
                VisualElement element = FindElementWithTag(tag);
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

        public VisualElement FindElementWithTag(string tag)
        {
            VisualElement element = root.Query(tag);
            return element;
        }

        public UXML_ControlledVisualElement GetUIElement(string tag)
        {
            if (uiElements.ContainsKey(tag))
            {
                return uiElements[tag];
            }
            return null;
        }

        void OnDestroy()
        {
            foreach (UXML_ControlledVisualElement element in uiElements.Values)
            {
                element.element.Clear();
            }
        }
    }
}