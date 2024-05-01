using System.Collections;
using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine;
using UnityEngine.UIElements;

public class GMItem : MonoBehaviour
{
    public VisualElement root;
    public VisualElement ItemImage;
    public Label ItemText;
    public VisualElement Item;
    // Start is called before the first frame update
    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        ItemImage = root.Q<VisualElement>("Changer");
        ItemText = root.Q<Label>("Text");
        Item = root.Q<VisualElement>("BG");
    }

    public void SetText(string text)
    {
        ItemText.text = text;
    }

    public void SetImage(Sprite image)
    {
        ItemImage.style.backgroundImage = new StyleBackground(image);
    }
    public void SetScale(float scale)
    {
        Item.style.scale = new StyleScale(new Vector2(scale, scale));
    }
}
