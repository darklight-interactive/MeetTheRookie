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
    public VisualElement Mystery;
    public VisualElement MysteryBG;
    public Label MysteryText;
    public GameObject Mystery1;
    public GameObject Mystery2;
    public GameObject SynthStack;
    public SynthStackUI CharacterStack;
    // Start is called before the first frame update
    void Start()
    {
        root = gameObject.GetComponent<UIDocument>().rootVisualElement;   
        stackbase = root.Q<VisualElement>("Base");
        Lupe = stackbase.Q<VisualElement>("lupe");
        lupesprite = new StyleBackground(sprite1);
        lupesprite2 = new StyleBackground(sprite2);
        Lupe.style.backgroundImage = lupesprite;
        Mystery = stackbase.Q<VisualElement>("Mystery");
        MysteryBG = Mystery.Q<VisualElement>("MysteryBG");
        MysteryText = MysteryBG.Q<Label>("MysteryTitle");

        if (Mystery1.activeInHierarchy)
        {
            CharacterStack = Mystery1.GetComponentInChildren<SynthStackUI>();
            MysteryText.text = "What Happened at the Old Winery?";
        }
        if (Mystery2.activeInHierarchy)
        {
            SynthStack = Mystery2.transform.GetChild(0).gameObject;
            CharacterStack = SynthStack.GetComponent<SynthStackUI>();
            CharacterStack.Move(-50, 15);
            CharacterStack.SetScale(1.4f, 1.4f);
            MysteryText.text = "Who Broke ito the Old Winery?";
            CharacterStack.AddText("Jenkins", "Claims he was at the Bar the night of the disturbance.");
            CharacterStack.AddText("Jenkins", "Claims that he doesn't go near the Winery anymore");
            CharacterStack.AddText("Teens", "Have never set foot inside the old Winery");
            CharacterStack.AddText("Teens", "Jenkins Tomm use to work at the Winery. He's 'crazy'");
            CharacterStack.AddText("Roy", "Suspects 'those troublemakers down at the Arcade.'");
            CharacterStack.AddText("Roy", "Has 'no idea' why anyone would want anything to do with the Winery");
        }

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
