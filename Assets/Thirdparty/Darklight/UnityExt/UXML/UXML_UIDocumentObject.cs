using System.Collections.Generic;
using Darklight.Game.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UnityExt.UXML
{
    /// <summary>
    /// A MonoBehaviour that handles the initialization of a UIDocument and manages its elements.
    /// It is suggested that you create a new ScriptableObject that inherits from UXML_UIDocumentPreset
    /// and assign it to the UIDocumentObject in the inspector.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public abstract class UXML_UIDocumentObject : MonoBehaviour
    {
        protected UXML_UIDocumentPreset preset;
        protected UIDocument document => GetComponent<UIDocument>();
        protected VisualElement root => document.rootVisualElement;
        protected Dictionary<string, UXML_CustomElement> uiElements = new Dictionary<string, UXML_CustomElement>();
        protected string[] elementTags = new string[0];

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
                    UXML_CustomElement elementWrapper = new UXML_CustomElement(element, tag);
                    uiElements.Add(tag, elementWrapper);
                }
                else
                {
                    Debug.LogWarning($"Element with tag {tag} not found in UIDocument {document.name}");
                }

                //Debug.Log($"Element with tag {tag} found in UIDocument {document.name}");
            }
        }

        public VisualElement FindElementWithTag(string tag)
        {
            VisualElement element = root.Query(tag);
            return element;
        }

        public UXML_CustomElement GetUIElement(string tag)
        {
            if (uiElements.ContainsKey(tag))
            {
                return uiElements[tag];
            }
            return null;
        }

        void OnDestroy()
        {
            foreach (UXML_CustomElement element in uiElements.Values)
            {
                element.Clear();
            }
        }
    }
}