using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneManagerScript : UXML_UIDocumentObject
{
    public VisualElement tscreen;
    VisualElement background;
    Label textlabel;
    public string newSceneName;
    string PreviousScene;
    // Start is called before the first frame update
    void Awake()
    {
        Initialize(SceneTransition.Instance.SceneManager, tags);
        tscreen = gameObject.GetComponent<UIDocument>().rootVisualElement;
        background = tscreen.Q<VisualElement>("blackborder");
        background.SetEnabled(false);
        textlabel = tscreen.Q<Label>("textlabel");
        textlabel.text = "";
        newSceneName = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (newSceneName != "")
        {
            PreviousScene = SceneManager.GetActiveScene().name;
            StartCoroutine(ChangeScene(newSceneName));
            newSceneName = "";
        }
        
    }
    IEnumerator ChangeScene(string newSceneName)
    {
        background.SetEnabled(true);
        yield return new WaitForSeconds(1);
        textlabel.text = newSceneName;
        yield return new WaitForSeconds(1);
        textlabel.text = "";
        yield return new WaitForSeconds(0.45f);
        SceneManager.LoadScene(newSceneName);
    }

}
