using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UXML
{
    public class UXML_WorldSpaceUI : UXML_UIDocumentObject
    {
        MeshRenderer _meshRenderer => GetComponentInChildren<MeshRenderer>();
        Material _material;
        RenderTexture _renderTexture;
        public bool isVisible => _meshRenderer.enabled;

        // -- Element Changed Event --
        public delegate void OnChange();
        protected OnChange OnElementChanged;
        void OnEnable()
        {
            OnElementChanged += TextureUpdate;
        }

        void OnDisable()
        {
            OnElementChanged -= TextureUpdate;
        }

        public void TextureUpdate()
        {
            // Set the material and texture
            _meshRenderer.sharedMaterial = new Material(_material); // << clone the material
            document.panelSettings.targetTexture = new RenderTexture(_renderTexture); // << set UIDocument target texture to clone
            _meshRenderer.sharedMaterial.mainTexture = document.panelSettings.targetTexture; // << set the material texture
            _meshRenderer.enabled = true;
        }

        public void Initialize(UXML_UIDocumentPreset preset, string[] tags, Material material, RenderTexture renderTexture)
        {
            _material = material;
            _renderTexture = renderTexture;
            base.Initialize(preset, tags);

            // Create a quad mesh child
            GameObject meshChild = GameObject.CreatePrimitive(PrimitiveType.Quad);
            meshChild.transform.SetParent(this.transform);
            meshChild.transform.localPosition = Vector3.zero;

            _meshRenderer.enabled = false;

            OnElementChanged?.Invoke();
        }

        public void SetLocalScale(float scale)
        {
            this.transform.localScale = new Vector3(scale, scale, scale);
        }

        public void Show()
        {
            _meshRenderer.enabled = true;
        }

        public void Hide()
        {
            _meshRenderer.enabled = false;
        }
    }
}
