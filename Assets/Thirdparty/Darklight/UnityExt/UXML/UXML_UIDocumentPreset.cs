using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UnityExt.UXML
{
    [CreateAssetMenu(menuName = "Darklight/UXML_UIDocumentPreset")]
    public class UXML_UIDocumentPreset : ScriptableObject
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private PanelSettings panelSettings;

        public VisualTreeAsset VisualTreeAsset => visualTreeAsset;
        public PanelSettings PanelSettings => panelSettings;
    }
}