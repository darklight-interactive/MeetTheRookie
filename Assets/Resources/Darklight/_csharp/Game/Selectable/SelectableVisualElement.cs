using System;
using Unity.Android.Gradle;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Darklight.Selectable
{
    /// <summary>
    /// Base class for controlled elements that encapsulates common functionalities for UXML elements.
    /// </summary>
    /// <typeparam name="TElement">The Type of the </typeparam>
    // Base class for controlled elements that encapsulates common functionalities for UXML elements.
    [UxmlElement]
    public partial class SelectableVisualElement<TElement> : VisualElement where TElement : VisualElement, new()
    {
        // Publicly accessible element instance.
        public TElement Element { get; private set; }
        public Rect Rect => Element.worldBound;
        public Vector2 CenterPosition => Element.worldBound.center;
        public SelectableVisualElement()
        {
            Element = new TElement();
            this.Add(Element);
            this.focusable = true;
        }

        public void Select()
        {
            this.AddToClassList("selected");
            this.Focus();
        }

        public void Deselect()
        {
            this.RemoveFromClassList("selected");
        }

        /// <summary>
        /// Allows implicit conversion from SelectableElement to the generic class, typeof(TElement) : VisualElement.
        /// </summary>
        /// <param name="wrapper"></param>
        public static implicit operator TElement(SelectableVisualElement<TElement> wrapper)
        {
            return wrapper.Element;
        }
    }

    // Specific implementation for a Button element.
    [UxmlElement]
    public partial class SelectableButton : SelectableVisualElement<Button>
    {
        public Button Button => Element as Button;
        public string Text
        {
            get => Element.text;
            set => Element.text = value;
        }
        public event Action OnClick;

        public SelectableButton()
        {
            Text = "selectable";
            Button.clickable.clicked += () => Click();
        }

        public void Click()
        {
            Button.RemoveFromClassList("selected");
            Button.AddToClassList("clicked");
            OnClick?.Invoke();
            Debug.Log("Button Clicked");
        }
    }

    [UxmlElement]
    public partial class SelectableSceneChangeButton : SelectableButton
    {
        [UxmlAttribute]
        public SceneAsset Scene { get; set; }
        public SelectableSceneChangeButton()
        {
            Text = "Change Scene";
            OnClick += () =>
            {
                if (Scene != null)
                {
                    SceneManager.LoadScene(Scene.name);
                }
            };
        }
    }
}
