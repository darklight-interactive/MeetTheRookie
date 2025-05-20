using UnityEngine;

public class MTRPrefabLibrary : ScriptableObject
{
    [Header("Camera")]
    public GameObject cameraPrefab;

    [Header("Interactable")]
    public Material spriteDefaultMaterial;
    public Material spriteOutlineMaterial;
    public Material spriteShimmerMaterial;

    [Header("UXML RenderTexture")]
    public Material uxmlRenderTextureMaterial;
    public RenderTexture uxmlRenderTexture;
}
