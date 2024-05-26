using Darklight.UXML;
using Darklight.UXML.Element;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This handles all of the Game UI elements like interactions and speech bubbles.
/// </summary>
public class GameUIController : UXML_UIDocumentObject
{
    const string INTERACT_PROMPT_TAG = "interact-icon";
    const string SPEECH_BUBBLE_TAG = "speech-bubble";

    VisualElement _header;
    VisualElement _body;
    VisualElement _footer;

    [Range(0.001f, 0.25f)]
    public float textScale = 0.01f;

    public void Start()
    {
        _body = ElementQuery<VisualElement>("body");
        _header = ElementQuery<VisualElement>("header");
        _footer = ElementQuery<VisualElement>("footer");

        ControlledLabel label = new ControlledLabel();
        _footer.Add(label);

        string fullText = "Hello, World! This is a long text string for a Controlled Size Text Element so that it can be tested with rolling text. Thank you for your patience.";

        Coroutine rollingTextRoutine = StartCoroutine(RollTextCoroutine(label, fullText, 0.01f));

        label.fontSize = UIManager.GetScreenWidth() * textScale;

        Debug.Log("Game UI Controller Initialized! Font Size: " + label.fontSize);
    }

    private IEnumerator RollTextCoroutine(ControlledLabel label, string fullText, float interval)
    {
        label.fullText = fullText;
        for (int i = 0; i < fullText.Length; i++)
        {
            label.rollingTextPercentage += interval;
            yield return new WaitForSeconds(interval);
        }

        // break when the text is fully rolled
        yield return null;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(GameUIController))]
public class GameUIControllerCustomEditor : Editor
{
    SerializedObject _serializedObject;
    GameUIController _script;

    public override void OnInspectorGUI()
    {

        _serializedObject = new SerializedObject(target);
        _script = (GameUIController)target;

        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Start"))
        {
            _script.Start();
        }

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif