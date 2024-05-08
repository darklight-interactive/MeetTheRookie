using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UXML;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InteractableNPC : Interactable
{
    [SerializeField] private float speechBubbleScalar = 1.5f;
    [SerializeField, ShowOnly] private UXML_WorldSpaceUI dialogueBubble;
    public UXML_WorldSpaceUI DialogueBubble { get => dialogueBubble; set => dialogueBubble = value; }

    public void Start()
    {
        NPC_Controller controller = GetComponent<NPC_Controller>();

        // >> ON INTERACTION -------------------------------------
        this.OnInteraction += (string currentText) =>
        {
            dialogueBubble = ShowDialogueBubble(currentText);
            if (controller)
            {
                controller.stateMachine.GoToState(NPCState.SPEAK);
            }
        };

        // >> ON COMPLETED -------------------------------------
        this.OnCompleted += () =>
        {
            //UIManager.WorldSpaceUI.Hide();
            if (controller)
            {
                controller.stateMachine.GoToState(NPCState.IDLE);
            }
        };
    }

    public UXML_WorldSpaceUI ShowDialogueBubble(string text)
    {
        //OverlapGrid2D_Data data = this.GetBestData();
        Vector3 position = this.transform.position;

        UXML_WorldSpaceUI worldSpaceUIDoc = UIManager.Instance.worldSpaceUI;
        worldSpaceUIDoc.transform.position = position;
        //worldSpaceUIDoc.transform.localScale = data.coordinateSize * Vector3.one * speechBubbleScalar;
        worldSpaceUIDoc.ElementQuery<Label>("inky-label").text = text;

        worldSpaceUIDoc.TextureUpdate();
        return worldSpaceUIDoc;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InteractableNPC))]
public class InteractableNPCCustomEditor : Editor
{
    SerializedObject _serializedObject;
    InteractableNPC _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (InteractableNPC)target;
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