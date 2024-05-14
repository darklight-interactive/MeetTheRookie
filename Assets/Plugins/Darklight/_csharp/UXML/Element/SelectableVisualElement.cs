using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Darklight.UXML.Element
{

    [UxmlElement]
    public partial class SelectableVisualElement : SelectableUIElement<VisualElement>, ISelectableUIElement
    {

    }

}
