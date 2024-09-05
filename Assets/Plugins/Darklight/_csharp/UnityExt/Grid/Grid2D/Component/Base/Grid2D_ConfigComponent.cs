using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Game.Grid
{
    public class Grid2D_ConfigComponent : Grid2D_BaseComponent
    {
        // ======== [[ FIELDS ]] ======================================================= >>>>
        [SerializeField, Expandable] Grid2D_ConfigDataObject _configObj;
        Grid2D_ConfigDataObject ConfigObj => _configObj;

        // ======== [[ METHODS ]] ======================================================= >>>>
        public override void OnUpdate()
        {
            base.OnUpdate();
            RefreshConfig();
        }

        public void CreateNewConfig()
        {
            string name = this.name + " Grid2D Config";

            _configObj = ScriptableObjectUtility.CreateOrLoadScriptableObject<Grid2D_ConfigDataObject>(Grid2D.DataObjectRegistry.CONFIG_PATH, name);
            _configObj.name = name;
        }

        void RefreshConfig()
        {
            if (_configObj == null) return;
            // Assign the grid's config from the config object
            Grid2D.Config newConfig = _configObj.CreateGridConfig();
            BaseGrid.SetConfig(newConfig);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Grid2D_ConfigComponent))]
        public class Grid2D_ConfigComponentCustomEditor : UnityEditor.Editor
        {
            SerializedObject _serializedObject;
            Grid2D_ConfigComponent _script;
            private void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (Grid2D_ConfigComponent)target;
                _script.Awake();
            }

            public override void OnInspectorGUI()
            {
                _serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                base.OnInspectorGUI();

                if (_script._configObj == null)
                {
                    if (GUILayout.Button("Create New Config"))
                    {
                        _script.CreateNewConfig();
                    }
                }

                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif

    }

}