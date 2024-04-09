using System;

using Darklight.UnityExt;
using Darklight.Game.Grid2D;

using Ink.Runtime;

using UnityEngine;
using UnityEngine.UIElements;

public class NPC_DialogueBubble
{
    [SerializeField, TextArea(3, 10)] public string inkyLabel = "Hello from the default settings on the NPC_DialogueBubble. >>";
    public Sprite bubbleSprite;
    public Material material_prefab;
    public RenderTexture renderTexture_prefab;
    public NPC_DialogueBubble() { }
    public NPC_DialogueBubble(string dialogueText, Sprite bubbleSprite, Material material_prefab, RenderTexture renderTexture_prefab)
    {
        this.inkyLabel = dialogueText;
        this.bubbleSprite = bubbleSprite;
        this.material_prefab = material_prefab;
        this.renderTexture_prefab = renderTexture_prefab;
    }
}

