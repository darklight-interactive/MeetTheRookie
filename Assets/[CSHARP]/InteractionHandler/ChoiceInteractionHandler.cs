using System.Collections.Generic;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using UnityEngine;
using NaughtyAttributes;
using Darklight.UnityExt.UXML;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChoiceInteractionHandler : Grid2D_OverlapWeightSpawner
{
    Dictionary<Vector2Int, UXML_RenderTextureObject> _attachedBubbles = new Dictionary<Vector2Int, UXML_RenderTextureObject>();
    [SerializeField] UXML_UIDocumentPreset _choiceBubblePreset;
    [SerializeField, Expandable] ChoiceBubbleLibrary _choiceBubbleLibrary;

    Material material => MTR_UIManager.Instance.UXML_RenderTextureMaterial;
    RenderTexture renderTexture => MTR_UIManager.Instance.UXML_RenderTexture;

    public override void OnInitialize(Grid2D grid)
    {
        if (_choiceBubbleLibrary == null)
        {
            _choiceBubbleLibrary = MTR_AssetManager.CreateOrLoadScriptableObject<ChoiceBubbleLibrary>();
        }
    }

    public void CreateBubbleAtAllCells(Sprite sprite)
    {
        List<Cell2D> cells = BaseGrid.GetCells();
        foreach (Cell2D cell in cells)
        {
            CreateBubbleAt(cell, sprite);
        }
    }

    void CreateBubbleAt(Cell2D cell, Sprite sprite)
    {
        Cell2D.SpawnerComponent spawnerComponent = cell.GetComponent<Cell2D.SpawnerComponent>();
        if (spawnerComponent != null)
        {
            UXML_RenderTextureObject spriteObject = UXML_Utility.CreateUXMLRenderTextureObject(_choiceBubblePreset, material, renderTexture);
            _attachedBubbles.Add(cell.Key, spriteObject);

            spawnerComponent.AttachTransformToCell(spriteObject.transform);
            //spriteObject.transform.localScale *= 0.1f;

            spriteObject.transform.SetParent(this.transform);

        }
    }

    void DestroyAllBubbles()
    {
        List<UXML_RenderTextureObject> bubbles = new List<UXML_RenderTextureObject>(_attachedBubbles.Values);
        for (int i = 0; i < bubbles.Count; i++)
        {
            ObjectUtility.DestroyAlways(bubbles[i].gameObject);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ChoiceInteractionHandler))]
    public class ChoiceInteractionHandlerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        ChoiceInteractionHandler _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (ChoiceInteractionHandler)target;
            _script.Awake();
        }


        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();


            if (_script._attachedBubbles.Count == 0 && GUILayout.Button("Create Default Sprite At All"))
            {
                //_script.CreateBubbleAtAllCells(_script._choiceBubbleLibrary.DefaultValue);
            }
            else if (_script._attachedBubbles.Count > 0 && GUILayout.Button("Destroy All Sprites"))
            {
                _script.DestroyAllBubbles();
            }


            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}


