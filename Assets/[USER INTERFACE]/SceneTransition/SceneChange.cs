using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SceneChange : MonoBehaviour
{
    public VisualElement tscreen;
    VisualElement background;
    Label textlabel;
    public bool condition = false;
    public string newSceneName;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        tscreen = GetComponent<UIDocument>().rootVisualElement;
        background = tscreen.Q<VisualElement>("background");
        background.SetEnabled(false);
        textlabel = tscreen.Q<Label>("textlabel");
        textlabel.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (condition == true)
        {
            background.style.backgroundColor = Color.black;
            StartCoroutine(ChangeScene(newSceneName));
            condition = false;
        }
        
    }
    IEnumerator ChangeScene(string newSceneName)
    {
        background.SetEnabled(true);
        yield return new WaitForSeconds(2);
        textlabel.text = newSceneName;
        UnityEngine.SceneManagement.SceneManager.LoadScene(newSceneName);
        yield return new WaitForSeconds(1);
        background.SetEnabled(false);
        yield return new WaitForSeconds(0.45f);
        textlabel.text = "";
        background.style.backgroundColor = Color.clear;
    }

}
