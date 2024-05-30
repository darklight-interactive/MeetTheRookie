using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UXML
{
    [CreateAssetMenu(menuName = "Darklight/UXML_UIDocumentPreset")]
    public class UXML_UIDocumentPreset : ScriptableObject
    {
        public VisualTreeAsset visualTreeAsset;
        public PanelSettings panelSettings;
    }
}