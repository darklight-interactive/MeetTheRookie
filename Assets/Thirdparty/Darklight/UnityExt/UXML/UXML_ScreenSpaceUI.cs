using Darklight.UnityExt.UXML;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.UXML
{
    /// <summary>
    /// Main class for the interaction UI. 
    /// This class is responsible for displaying all Screen Space UI 
    /// elements related to interaction.
    /// </summary>
    public class UXML_ScreenSpaceUI : UXML_UIDocumentObject
    {
        const string PROMPT_TAG = "interactPrompt";
        const string CHOICE_GROUP_TAG = "choiceGroup";

        [SerializeField] private UXML_UIDocumentPreset _preset;

        public virtual void Awake()
        {
            elementTags = new string[] { PROMPT_TAG, CHOICE_GROUP_TAG };
            Initialize(_preset, elementTags);
        }

        public void DisplayInteractPrompt(Vector3 worldPosition)
        {
            UXML_CustomElement uIElement = GetUIElement(PROMPT_TAG);
            if (uIElement == null) return;
            uIElement.SetWorldToScreenPosition(worldPosition);
            uIElement.SetVisible(true);
        }

        public void HideInteractPrompt()
        {
            UXML_CustomElement uIElement = GetUIElement(PROMPT_TAG);
            if (uIElement == null) return;
            uIElement.SetVisible(false);
        }


        /*
            public void MoveUpdate(Vector2 move)
            {
                if (!handlingChoice)
                {
                    return;
                }
                float x = Mathf.Sign(move.x);
                float y = -Mathf.Sign(move.y);

                int choice = activeChoice;
                if (Mathf.Abs(move.x) > 0.05f)
                {
                    choice = (int)Mathf.Clamp(activeChoice + x, 0, story.currentChoices.Count - 1);
                }
                else if (Mathf.Abs(move.y) > 0.05f)
                {
                    choice = (int)Mathf.Clamp(activeChoice + y, 0, story.currentChoices.Count - 1);
                }
                UpdateActiveChoice(choice);
            }
        */

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UXML_ScreenSpaceUI))]
    public class CustomEditorForScript : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        UXML_ScreenSpaceUI _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (UXML_ScreenSpaceUI)target;

            _script.Awake();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}
