using Darklight.UnityExt.Library;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Darklight/Grid2D/WeightDataObject")]
public class Grid2D_WeightDataObject : ScriptableLibrary<Vector2Int, int, Library<Vector2Int, int>>
{
    [SerializeField]
    Library<Vector2Int, int> _dataLibrary = new Library<Vector2Int, int>
    {
        ReadOnlyKey = true,
        ReadOnlyValue = false,
        RequiredKeys = new Vector2Int[0],
    };

    public override Library<Vector2Int, int> DataLibrary
    {
        get
        {
            if (_dataLibrary == null)
            {
                _dataLibrary = new Library<Vector2Int, int>
                {
                    ReadOnlyKey = true,
                    ReadOnlyValue = false,
                    RequiredKeys = new Vector2Int[0],
                };
            }
            else
            {
                _dataLibrary.ReadOnlyKey = true;
                _dataLibrary.ReadOnlyValue = false;
            }
            return _dataLibrary;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Grid2D_WeightDataObject))]
    public class Grid2D_WeightDataObjectCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        Grid2D_WeightDataObject _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (Grid2D_WeightDataObject)target;
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (GUILayout.Button("SetValuesToDefault"))
            {
                _script.SetValuesToDefault();
            }

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
                _script._dataLibrary.Refresh();
            }
        }
    }
#endif
}



