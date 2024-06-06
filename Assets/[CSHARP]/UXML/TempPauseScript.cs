using UnityEngine;
using UnityEngine.UIElements;

public class TempPauseScript : MonoBehaviour
{
    private UIDocument flicker;
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
