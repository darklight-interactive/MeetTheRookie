using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    private UIDocument flicker;
    private VisualElement SoundsMenu;
    private VisualElement ControlsMenu;
    private VisualElement GeneralMenu;
    private List<VisualElement> Interactors = new List<VisualElement>();
    // Start is called before the first frame update
    void Start()
    {  
        flicker = GetComponent<UIDocument>();
        flicker.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            flicker.enabled = !flicker.enabled;
        }
        if (flicker.enabled == true)
        {
            Time.timeScale = 0;
        }
        if (flicker.enabled == false)
        {
            Time.timeScale = 1;
        }
    }
}
