using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "MeetTheRookie/UXML_UIDocumentPreset")]
public class UXML_UIDocumentPreset : ScriptableObject
{
    [SerializeField] private VisualTreeAsset visualTreeAsset;
    [SerializeField] private PanelSettings panelSettings;

    public VisualTreeAsset VisualTreeAsset => visualTreeAsset;
    public PanelSettings PanelSettings => panelSettings;
}