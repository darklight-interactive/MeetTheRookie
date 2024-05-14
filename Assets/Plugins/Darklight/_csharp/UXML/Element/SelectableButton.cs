using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Darklight.UXML.Element
{
    #region ---- [[ SELECTABLE BUTTON ]] ----
    // Specific implementation for a Button element.
    [UxmlElement]
    public partial class SelectableButton : SelectableVisualElement<Button>
    {
        public new class UxmlFactory : UxmlFactory<SelectableButton> { }


        [UxmlAttribute]
        public string Text
        {
            get { return Element.text; }
            set { Element.text = value; }
        }

        public SelectableButton()
        {
            Element = new Button();
            Element.text = Text;
            this.Add(Element);
            Element.clickable.clicked += ClickAction;
        }

        private void ClickAction()
        {
            Click();
            Element.clickable.clicked -= ClickAction;
        }
    }
    #endregion

    #region ---- [[ SELECTABLE SCENE CHANGE BUTTON ]] ----
    [UxmlElement]
    public partial class SelectableSceneChangeButton : SelectableButton
    {
        [UxmlAttribute]
        public SceneAsset scene;
        public SelectableSceneChangeButton()
        {
            OnClick += ChangeScene;
        }

        private void ChangeScene()
        {
            if (scene != null)
            {
                SceneManager.LoadScene(scene.name);
                OnClick -= ChangeScene; // Ensure this is only called once
            }
        }
    }
    #endregion
}