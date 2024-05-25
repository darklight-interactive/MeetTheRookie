using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt;
using Darklight.UnityExt.Editor;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UXML
{

    /// <summary>
    /// This class is used to create a GameObject with a RenderTexture that can be used to render a UXML Element.
    /// </summary>
    public class UXML_RenderTextureObject : UXML_UIDocumentObject
    {
        [SerializeField, ShowOnly] GameObject _quad;
        [SerializeField, ShowOnly] MeshRenderer _meshRenderer;
        [SerializeField, ShowOnly] Material _material;
        [SerializeField, ShowOnly] RenderTexture _renderTexture;

        // -- Element Changed Event --
        public delegate void OnChange();
        protected OnChange OnElementChanged;

        public void Initialize(UXML_UIDocumentPreset preset, string[] tags, Material material, RenderTexture renderTexture)
        {
            _material = material;
            _renderTexture = renderTexture;
            base.Initialize(preset, tags);

            // Create a quad mesh child
            _quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            _quad.transform.SetParent(this.transform);
            _quad.transform.localPosition = Vector3.zero;
            _meshRenderer = _quad.GetComponent<MeshRenderer>();

            gameObject.layer = LayerMask.NameToLayer("UI");
            _quad.layer = LayerMask.NameToLayer("UI");

            OnElementChanged?.Invoke();
        }

        public void TextureUpdate()
        {
            // Set the material and texture
            _meshRenderer.sharedMaterial = new Material(_material); // << clone the material
            document.panelSettings.targetTexture = new RenderTexture(_renderTexture); // << set UIDocument target texture to clone
            _meshRenderer.sharedMaterial.mainTexture = document.panelSettings.targetTexture; // << set the material texture
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
    }
}
