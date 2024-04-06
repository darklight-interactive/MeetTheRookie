using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class DynamicTextBubble : MonoBehaviour
{
    public TextMeshProUGUI TMPDialogue;

    [TextArea(3, 10)]
    public string text;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TMPDialogue.text = text;
    }
}
