using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UXML_SceneTransition : UXML_UIDocumentObject
{
    UXML_Element blackborder_docElement;
    public VisualElement tscreen;
    VisualElement background;
    Label textlabel;

    public bool isStarted;
    public bool isComplete;

    // Start is called before the first frame update
    public override void Initialize(UXML_UIDocumentPreset preset, string[] tags = null)
    {
        // Initialize the base class
        base.Initialize(preset, tags);

        // Get the root element of the document
        tscreen = base.root;


        blackborder_docElement = base.GetUIElement("blackborder");
        background = blackborder_docElement.visualElement;
        blackborder_docElement.SetVisible(false);

        textlabel = tscreen.Q<Label>("textlabel");
        textlabel.text = "";
    }

    public void BeginFadeOut(string newSceneName)
    {
        isStarted = true;
        isComplete = false;
        StartCoroutine(FadeOut(newSceneName));
    }

    public void BeginFadeIn()
    {
        isStarted = true;
        isComplete = false;
        //StartCoroutine(FadeIn()); TODO
    }


    IEnumerator FadeOut(string newSceneName)
    {
        blackborder_docElement.SetVisible(true);
        background.SetEnabled(true);
        textlabel.SetEnabled(true);

        yield return new WaitForSeconds(1);
        textlabel.visible = true;
        textlabel.text = newSceneName;
        yield return new WaitForSeconds(1);
        textlabel.visible = false;
        yield return new WaitForSeconds(0.45f);

        background.SetEnabled(false);
        background.visible = false;

        textlabel.SetEnabled(false);

        UnityEngine.SceneManagement.SceneManager.LoadScene(newSceneName);
    }
}
