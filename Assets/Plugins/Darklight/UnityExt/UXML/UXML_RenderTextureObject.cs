using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt;
using Darklight.UnityExt.Editor;
using UnityEngine.UI;




#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.UXML
{

    /// <summary>
    /// This class is used to create a GameObject with a RenderTexture that can be used to render a UXML Element.
    /// </summary>
    public class UXML_RenderTextureObject : UXML_UIDocumentObject, IUnityEditorListener
    {
        Material _materialInstance;
        RenderTexture _renderTextureInstance;


        Material _lastMaterial;
        RenderTexture _lastRenderTexture;

        [SerializeField, ShowOnly] GameObject _quad;
        [SerializeField, ShowOnly] MeshRenderer _meshRenderer;
        [SerializeField] Material _material;
        [SerializeField] RenderTexture _renderTexture;

        // -- Element Changed Event --
        public void OnEditorReloaded()
        {
#if UNITY_EDITOR
            DestroyImmediate(this.gameObject);
#endif
        }

        public override void Initialize(UXML_UIDocumentPreset preset, string[] tags = null)
        {
            this.Initialize(preset, tags, _material, _renderTexture);
        }

        public void Initialize(UXML_UIDocumentPreset preset, string[] tags, Material material, RenderTexture renderTexture)
        {
            _material = material;
            _renderTexture = renderTexture;
            base.Initialize(preset, tags);

            // Create a quad mesh child
            if (_quad == null || _meshRenderer == null)
            {
                _quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                _quad.transform.SetParent(this.transform);
                _quad.transform.localPosition = Vector3.zero;
                _meshRenderer = _quad.GetComponent<MeshRenderer>();

                gameObject.layer = LayerMask.NameToLayer("UI");
                _quad.layer = LayerMask.NameToLayer("UI");
            }

            // Create a new material instance
            if (_materialInstance == null)
                _materialInstance = new Material(_material);

            // Create a new render texture instance
            if (_renderTextureInstance == null)
                _renderTextureInstance = new RenderTexture(_renderTexture);

            // Assign the render texture to the panel settings and material
            document.panelSettings.targetTexture = _renderTextureInstance;
            _materialInstance.mainTexture = _renderTextureInstance;

            // Assign the material to the mesh renderer
            _meshRenderer.sharedMaterial = _materialInstance;

            _lastMaterial = _materialInstance;
            _lastRenderTexture = _renderTextureInstance;

            OnInitialized();
        }

        protected virtual void OnInitialized() { }


        public void TextureUpdate()
        {
            if (_renderTextureInstance == null || _materialInstance == null)
            {
                Debug.LogWarning("Material or RenderTexture instance is not initialized.");
                return;
            }

            RenderTexture cloneTexture = new RenderTexture(_lastRenderTexture);
            document.panelSettings.targetTexture = cloneTexture;
            _lastRenderTexture = cloneTexture;

            _meshRenderer.sharedMaterial = new Material(_lastMaterial);
            _meshRenderer.sharedMaterial.mainTexture = cloneTexture;
            _lastMaterial = _meshRenderer.sharedMaterial;

            // Force the UI document to repaint
            document.rootVisualElement.MarkDirtyRepaint();
        }

        public void SetLocalScale(float scale)
        {
            this.transform.localScale = new Vector3(scale, scale, scale);
        }

        public void Destroy()
        {
            if (Application.isPlaying)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DestroyImmediate(this.gameObject);
            }
        }

        void OnDrawGizmosSelected()
        {
            CustomGizmos.DrawWireRect(this.transform.position, this.transform.localScale, Vector3.forward, Color.white);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(UXML_RenderTextureObject), true)]
        public class UXML_RenderTextureObjectCustomEditor : UXML_UIDocumentObjectCustomEditor
        {
            SerializedObject _serializedObject;
            UXML_RenderTextureObject _script;
            private void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (UXML_RenderTextureObject)target;
            }

            public override void OnInspectorGUI()
            {
                _serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                base.OnInspectorGUI();

                if (GUILayout.Button("Update Texture"))
                {
                    _script.TextureUpdate();
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
