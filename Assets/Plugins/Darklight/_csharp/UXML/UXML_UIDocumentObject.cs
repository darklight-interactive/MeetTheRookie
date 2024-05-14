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
    public class UXML_UIDocumentObject : MonoBehaviour, IUnityEditorListener
    {
        public void OnEditorReloaded()
        {
            DestroyImmediate(this);
        }
        // << SERIALIZED VALUES >> //
        [SerializeField] UXML_UIDocumentPreset _preset;

        // << PUBLIC ACCESSORS >> //
        public UXML_UIDocumentPreset preset => _preset;
        public UIDocument document => GetComponent<UIDocument>();
        public VisualElement root => document.rootVisualElement;
        public Dictionary<string, VisualElement> uiElements { get; private set; } = new();
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

        /// <summary>
        /// Sets the position of a UI Toolkit element to correspond to a world position.
        /// Optionally centers the element on the screen position.
        /// </summary>
        /// <param name="element">The UI Toolkit element to position.</param>
        /// <param name="worldPosition">The world position to map to screen space.</param>
        /// <param name="center">Optional parameter to center the element at the screen position (default false).</param>
        public void SetWorldToScreenPoint(VisualElement element, Vector3 worldPosition, bool center = false)
        {
            Camera cam = Camera.main;
            if (cam == null) throw new System.Exception("No main camera found.");

            // Convert world position to screen position
            Vector3 screenPosition = cam.WorldToScreenPoint(worldPosition);
            screenPosition.y = cam.pixelHeight - screenPosition.y;  // UI Toolkit uses top-left origin
            screenPosition.z = 0;

            if (center)
            {
                // Adjust position to center the element
                screenPosition.x -= element.resolvedStyle.width / 2;
                screenPosition.y -= element.resolvedStyle.height / 2;
            }

            // Set positions using left and top in style
            element.style.left = screenPosition.x;
            element.style.top = screenPosition.y;
        }
    }
}