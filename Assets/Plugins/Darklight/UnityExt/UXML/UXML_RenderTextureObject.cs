using System.Collections;
using Darklight.UnityExt.Editor;
using UnityEngine;

namespace Darklight.UnityExt.UXML
{
    /// <summary>
    /// This class is used to create a GameObject with a RenderTexture that can be used to render a UXML Element.
    /// </summary>
    public class UXML_RenderTextureObject : UXML_UIDocumentObject, IUnityEditorListener
    {
        private RenderTexture _backBuffer;
        private RenderTexture _frontBuffer;
        private Material _materialInstance;

        [SerializeField, ShowOnly]
        private GameObject _quad;

        [SerializeField, ShowOnly]
        private MeshRenderer _meshRenderer;

        [SerializeField]
        private Material _material;

        [SerializeField]
        private RenderTexture _renderTexture;

        // -- Element Changed Event --
        public void OnEditorReloaded()
        {
#if UNITY_EDITOR
            DestroyImmediate(this.gameObject);
#endif
        }

        public void Initialize(
            UXML_UIDocumentPreset preset,
            Material material,
            RenderTexture renderTexture,
            bool clonePanelSettings = false
        )
        {
            _material = material;
            _renderTexture = renderTexture;
            base.Initialize(preset, clonePanelSettings);

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

            // Initialize front and back buffers
            // Initialize front and back buffers
            if (_renderTexture == null)
                return;

            // Pick a safe format for this platform
            var safeFormat = GetSupportedRenderTextureFormat();
            _backBuffer = new RenderTexture(_renderTexture)
            {
                format = safeFormat,
                enableRandomWrite = false
            };
            _frontBuffer = new RenderTexture(_renderTexture)
            {
                format = safeFormat,
                enableRandomWrite = false
            };

            // Create a new material instance
            _materialInstance = new Material(_material);

            // Assign the front buffer to the panel settings and material initially
            document.panelSettings.targetTexture = _frontBuffer;
            _materialInstance.mainTexture = _frontBuffer;

            // Assign the material to the mesh renderer
            _meshRenderer.sharedMaterial = _materialInstance;

            OnInitialized();
        }

        protected virtual void OnInitialized() { }

        public void FixedUpdate()
        {
            // Only call TextureUpdate if necessary
            if (root.resolvedStyle.width > 0 && root.resolvedStyle.height > 0)
            {
                TextureUpdate();
            }
        }

        void TextureUpdate()
        {
            StartCoroutine(TextureUpdateRoutine());
        }

        RenderTextureFormat GetSupportedRenderTextureFormat()
        {
            // Preferred order of formats (higher quality to lower)
            RenderTextureFormat[] preferredFormats = new[]
            {
                RenderTextureFormat.ARGBHalf,
                RenderTextureFormat.DefaultHDR,
                RenderTextureFormat.Default,
                RenderTextureFormat.ARGB32
            };

            foreach (var format in preferredFormats)
            {
                if (SystemInfo.SupportsRenderTextureFormat(format))
                    return format;
            }

            // Final fallback
            return RenderTextureFormat.ARGB32;
        }

        IEnumerator TextureUpdateRoutine()
        {
            // Render to the back buffer
            RenderToBackBuffer();

            yield return new WaitForEndOfFrame();

            // Swap buffers
            SwapBuffers();

            // Update the material to use the new front buffer
            _meshRenderer.sharedMaterial.mainTexture = _frontBuffer;

            // Update the panel settings with the new front buffer
            document.panelSettings.targetTexture = _frontBuffer;

            // Force the UI document to repaint
            document.rootVisualElement.MarkDirtyRepaint();
        }

        private void RenderToBackBuffer()
        {
            if (_backBuffer == null)
            {
                Debug.LogWarning("Back buffer is not initialized.");
                return;
            }

            // Set the back buffer as the target for rendering
            document.panelSettings.targetTexture = _backBuffer;

            // Clear the back buffer
            RenderTexture.active = _backBuffer;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;

            // Force the UI document to repaint and render onto the back buffer
            document.rootVisualElement.MarkDirtyRepaint();

            // Ensure the UI rendering occurs
            //UIElementsUtility.UpdatePanels();
        }

        private void SwapBuffers()
        {
            // Swap the front and back buffers
            var temp = _frontBuffer;
            _frontBuffer = _backBuffer;
            _backBuffer = temp;
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

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            CustomGizmos.DrawWireRect(
                this.transform.position,
                this.transform.localScale,
                Vector3.forward,
                Color.white
            );
        }
#endif
    }
}
