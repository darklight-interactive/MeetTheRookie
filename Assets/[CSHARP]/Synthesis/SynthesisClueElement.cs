using System.Collections.Generic;
using Darklight.UnityExt.UXML;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SynthesisClueElement : ControlledLabel
{
    public new class UxmlFactory : UxmlFactory<SynthesisClueElement> { }

    const string DEFAULT_BACKGROUND = "Assets/Resources/SpriteDefaults/paper_medium2.png";

    [UxmlAttribute]
    public Sprite backgroundSprite
    {
        get { return this.style.backgroundImage.value.sprite; }
        set { SetBackgroundSprite(value); }
    }

    public SynthesisClueElement()
    {
        this.fullText = "New Synthesis Clue";
        this.rollingTextPercentage = 1;
        //backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(DEFAULT_BACKGROUND);

        this.style.justifyContent = Justify.Center;

        AddToClassList("synthesis-object");
    }

    public void SetBackgroundSprite(Sprite sprite)
    {
        if (sprite == null)
            this.style.backgroundImage = null;
        else
            this.style.backgroundImage = new StyleBackground(sprite);
    }
}
