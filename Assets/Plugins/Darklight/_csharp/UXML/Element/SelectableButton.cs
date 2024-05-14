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
        private Button Button
        {
            get => Element;
        }

        [UxmlAttribute]
        public string text
        {
            get => Button.text;
            set => Button.text = value;
        }

        public SelectableButton() : base()
        {
            Button.text = "selectable-button";
            Button.clickable.clicked += ClickAction;
        }

        private void ClickAction()
        {
            Click();
            Button.clickable.clicked -= ClickAction;
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