using System.Collections.Generic;

using Darklight.UnityExt.Behaviour.Interface;
using Darklight.UnityExt.Editor;

using UnityEngine;
using System;
using NaughtyAttributes;
using Darklight.UnityExt.Utility;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Core2D
{
    [ExecuteAlways]
    public partial class Grid2D : MonoBehaviour, IUnityEditorListener
    {
        // ======== [[ CONSTANTS ]] ======================================================= >>>>

        protected const string CONSOLE_PREFIX = "[GRID2D]";

        // ======== [[ FIELDS ]] ======================================================= >>>>
        ConsoleGUI _consoleGUI = new ConsoleGUI();
        Dictionary<Vector2Int, Cell2D> _cellMap;
        ComponentRegistry _componentRegistry;

        [Header("States")]
        [SerializeField, ShowOnly] bool _isLoaded = false;
        [SerializeField, ShowOnly] bool _isInitialized = false;

        [Header("Gizmos")]
        [SerializeField] bool _showGizmos;

        [Space(5), Header("Config")]
        [SerializeField] Config _config;
        [SerializeField, Expandable] Grid2D_ConfigDataObject _configObj;

        [Space(5), Header("Cells")]
        [SerializeField] List<Cell2D> _cellsInMap;


        // ======== [[ PROPERTIES ]] ======================================================= >>>>
        protected ConsoleGUI GridConsole => _consoleGUI;

        public bool ShowGizmos => _showGizmos;
        public Config Configuration
        {
            get
            {
                if (_config == null)
                    _config = new Config();
                return _config;
            }
            protected set { _config = value; }
        }
        public Dictionary<Vector2Int, Cell2D> CellMap
        {
            get
            {
                if (_cellMap == null)
                    _cellMap = new Dictionary<Vector2Int, Cell2D>();
                return _cellMap;
            }
            protected set { _cellMap = value; }
        }
        public HashSet<Vector2Int> CellKeys => new HashSet<Vector2Int>(CellMap.Keys);
        public HashSet<Cell2D> CellValues => new HashSet<Cell2D>(CellMap.Values);

        // -- (( VISITORS )) ------------------ >>
        protected Cell2D.Visitor CellUpdateVisitor => new Cell2D.Visitor(cell =>
        {
            cell.RecalculateDataFromGrid(this);
            cell.Update();
            return true;
        });

        // ======== [[ EVENTS ]] ======================================================= >>>>
        public delegate void GridEvent();
        public event GridEvent OnGridPreloaded;
        public event GridEvent OnGridInitialized;
        public event GridEvent OnGridUpdated;

        // ======== [[ PUBLIC METHODS ]] ============================================================ >>>>
        public void OnEditorReloaded()
        {
            _consoleGUI.Clear();
            Reset();
        }

        #region -- (( UNITY RUNTIME )) -------- )))
        public void Awake() => Preload();

        public void Start() => Initialize();

        public void Update() => Refresh();
        #endregion

        #region -- (( VISITOR PATTERN )) -------- )))
        public void SendVisitorToCell(Vector2Int key, IVisitor<Cell2D> visitor)
        {
            if (CellMap == null) return;

            // Skip if the key is not in the map
            if (!CellMap.ContainsKey(key)) return;

            // Apply the map function to the cell
            Cell2D cell = CellMap[key];
            cell.Accept(visitor);
        }

        public void SendVisitorToAllCells(IVisitor<Cell2D> visitor)
        {
            if (CellMap == null) return;

            List<Vector2Int> keys = new List<Vector2Int>(CellMap.Keys);
            foreach (Vector2Int key in keys)
            {
                // Skip if the key is not in the map
                if (!CellMap.ContainsKey(key)) continue;

                // Apply the map function to the cell
                Cell2D cell = CellMap[key];
                cell.Accept(visitor);
            }
        }
        #endregion

        #region -- (( GETTERS )) -------- )))
        public Config GetConfig()
        {
            if (_config == null)
                _config = new Config();
            return _config;
        }

        public List<Cell2D> GetCells()
        {
            return new List<Cell2D>(CellMap.Values);
        }

        public Cell2D GetCell(Vector2Int key)
        {
            if (CellMap.ContainsKey(key))
                return CellMap[key];
            return null;
        }

        public List<Cell2D> GetCellsByComponentType(ComponentTypeKey type)
        {
            List<Cell2D> cells = new List<Cell2D>();
            foreach (Cell2D cell in CellMap.Values)
            {
                if (cell.ComponentReg.HasComponent(type))
                {
                    cells.Add(cell);
                }
            }
            return cells;
        }

        public List<TComponent> GetComponentsByType<TComponent>()
            where TComponent : Cell2D.Component
        {
            List<TComponent> components = new List<TComponent>();
            foreach (Cell2D cell in CellMap.Values)
            {
                TComponent component = cell.ComponentReg.GetComponent<TComponent>();
                if (component != null)
                {
                    components.Add(component);
                }
            }
            return components;
        }

        public Cell2D GetClosestCellTo(Vector2 position)
        {
            Cell2D closestCell = null;
            float closestDistance = float.MaxValue;
            foreach (Cell2D cell in CellMap.Values)
            {
                float distance = Vector2.Distance(cell.Position, position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCell = cell;
                }
            }
            return closestCell;
        }
        #endregion

        #region -- (( SETTERS )) -------- )))

        public void SetCells(List<Cell2D> cells)
        {
            if (cells == null || cells.Count == 0) return;
            foreach (Cell2D cell in cells)
            {
                if (cell == null) continue;
                if (CellMap.ContainsKey(cell.Key))
                    CellMap[cell.Key] = cell;
                else
                    CellMap.Add(cell.Key, cell);
            }
        }

        public void Reset()
        {
            _isLoaded = false;
            _isInitialized = false;
            _consoleGUI.Clear();

            Initialize();
        }
        #endregion

        #region -- (( RUNTIME )) -------- )))
        void Preload()
        {
            _isLoaded = false;
            _isInitialized = false;

            // Create a new config if none exists
            if (_config == null)
            {
                _config = new Config();
                if (_configObj != null)
                    _configObj.UpdateConfig(_config);
            }

            // Create a new cell map
            _cellMap = new Dictionary<Vector2Int, Cell2D>();

            // Create a new component system
            _componentRegistry = new ComponentRegistry(this);

            // Determine if the grid was preloaded
            _isLoaded = true;

            // Invoke the grid preloaded event
            OnGridPreloaded?.Invoke();
            GridConsole.Log($"Preloaded: {_isLoaded}");
        }

        void Initialize()
        {
            if (!_isLoaded) Preload();

            // Generate a new grid from the config
            bool mapGenerated = GenerateCellMap();

            // Determine if the grid was initialized
            _isInitialized = mapGenerated;

            // Return if the grid was not initialized
            if (!_isInitialized) return;

            // Invoke the grid initialized event
            OnGridInitialized?.Invoke();
            GridConsole.Log($"Initialized: {_isInitialized}");
        }

        void Refresh()
        {
            // Initialize if not already
            if (!_isInitialized || _config == null)
            {
                Initialize();
                return;
            }

            // Update the config if the data object is not null
            if (_configObj != null)
            {
                _configObj.UpdateConfig(_config);
                _config.UpdateTransformData(this.transform);
            }

            // Resize the grid if the dimensions have changed
            ResizeCellMap();

            _cellsInMap = new List<Cell2D>(CellMap.Values);

            // Update the cells
            SendVisitorToAllCells(CellUpdateVisitor);
            OnGridUpdated?.Invoke();
        }

        void Clear()
        {
            _isLoaded = false;
            _isInitialized = false;

            if (CellMap != null)
                CellMap.Clear(); // << Clear the map

            GridConsole.Log("Cleared");
        }
        #endregion

        #region -- (( HANDLE CELLS )) -------- )))
        bool CreateCell(Vector2Int key)
        {
            if (_cellMap.ContainsKey(key))
                return false;

            Cell2D cell = (Cell2D)Activator.CreateInstance(typeof(Cell2D), key);
            _cellMap[key] = cell;
            return true;
        }

        bool RemoveCell(Vector2Int key)
        {
            if (!_cellMap.ContainsKey(key))
                return false;

            _cellMap.Remove(key);
            return true;
        }

        bool GenerateCellMap()
        {
            // Skip if already initialized
            if (_isInitialized) return false;

            // Clear the map
            _cellMap.Clear();

            // Iterate through the grid dimensions and create cells
            Vector2Int dimensions = Configuration.GridDimensions;
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    Vector2Int gridKey = new Vector2Int(x, y);
                    CreateCell(gridKey);
                }
            }

            if (_cellMap.Count == 0) return false;
            return true;
        }

        void ResizeCellMap()
        {
            if (!_isInitialized) return;
            Vector2Int newDimensions = Configuration.GridDimensions;

            // Check if the dimensions have changed
            int newGridArea = newDimensions.x * newDimensions.y;
            int oldGridArea = CellMap.Count;
            if (newGridArea == oldGridArea) return;

            // Remove cells that are out of bounds
            List<Vector2Int> keys = new List<Vector2Int>(CellMap.Keys);
            foreach (Vector2Int key in keys)
            {
                if (key.x >= newDimensions.x || key.y >= newDimensions.y)
                    RemoveCell(key);
            }

            // Add cells that are in bounds
            for (int x = 0; x < newDimensions.x; x++)
            {
                for (int y = 0; y < newDimensions.y; y++)
                {
                    Vector2Int gridKey = new Vector2Int(x, y);
                    CreateCell(gridKey);
                }
            }
        }
        #endregion

#if UNITY_EDITOR
        Grid2D_ConfigDataObject CreateNewConfigDataObject()
        {
            string name = this.name;
            _configObj = ScriptableObjectUtility.CreateOrLoadScriptableObject<Grid2D_ConfigDataObject>(Grid2D.DataObjectRegistry.CONFIG_PATH, name);
            _configObj.name = name;
            return _configObj;
        }
#endif

        // ======== [[ NESTED TYPES ]] ======================================================= >>>>
        public enum Alignment
        {
            TopLeft, TopCenter, TopRight,
            MiddleLeft, Center, MiddleRight,
            BottomLeft, BottomCenter, BottomRight
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Grid2D), true)]
        public class Grid2D_Editor : UnityEditor.Editor
        {
            protected SerializedObject _serializedObject;
            Grid2D _script;

            protected virtual void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (Grid2D)target;

                _script.GridConsole.Clear();
                _script.Reset();
            }

            public override void OnInspectorGUI()
            {
                _serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.Space();

                // < DEFAULT INSPECTOR > ------------------ >>
                CustomInspectorGUI.DrawDefaultInspectorWithoutSelfReference(_serializedObject);

                // < CUSTOM INSPECTOR > ------------------ >>
                CustomInspectorGUI.DrawHorizontalLine(Color.gray, 4, 10);
                if (GUILayout.Button("Initialize")) { _script.Initialize(); }

                if (_script._configObj == null)
                {
                    if (GUILayout.Button("Create New Config"))
                    {
                        _script.CreateNewConfigDataObject();
                    }
                }

                // < CONSOLE > ------------------ >>
                CustomInspectorGUI.DrawHorizontalLine(Color.gray, 4, 10);
                _script._consoleGUI.DrawInEditor();

                // Apply changes if any
                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                    //Repaint();
                    _script.Refresh();
                }
            }

            void OnSceneGUI()
            {
                //_script.Refresh();
            }
        }
#endif
    }


}
