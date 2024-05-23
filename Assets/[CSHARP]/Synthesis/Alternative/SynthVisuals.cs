using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SynthVisuals : MonoBehaviour
{
    public VisualElement root;
    public VisualElement stackbase;
    public VisualElement Lupe;
    public Sprite sprite1;
    public Sprite sprite2;
    public StyleBackground lupesprite;
    public StyleBackground lupesprite2;
    // Start is called before the first frame update
    void Start()
    {
        root = gameObject.GetComponent<UIDocument>().rootVisualElement;   
        stackbase = root.Q<VisualElement>("Base");
        Lupe = stackbase.Q<VisualElement>("lupe");
        lupesprite = new StyleBackground(sprite1);
        lupesprite2 = new StyleBackground(sprite2);
        Lupe.style.backgroundImage = lupesprite;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSprite();
    }
    IEnumerator LupeAnimator()
    {
        yield return new WaitForSeconds(0.25f);
    }
    void ChangeSprite()
    {
        if (Lupe.style.backgroundImage == lupesprite)
        {
            Lupe.style.backgroundImage = lupesprite2;
            StartCoroutine(LupeAnimator());
        }
        if (Lupe.style.backgroundImage == lupesprite2)
        {
            Lupe.style.backgroundImage = lupesprite;
            StartCoroutine(LupeAnimator());
        }
    }
}
