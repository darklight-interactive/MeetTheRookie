using System.Collections;
using Darklight.UnityExt.UXML;
using UnityEngine;
using UnityEngine.UIElements;

public class UXML_SceneTransition : UXML_UIDocumentObject
{
    UXML_CustomElement blackOverlayElement;
    Label textlabel;

    public bool isStarted;
    public bool isComplete;

    // Start is called before the first frame update
    public override void Initialize(UXML_UIDocumentPreset preset, string[] tags = null)
    {
        // Initialize the base class
        base.Initialize(preset, tags);

        textlabel = FindElementWithTag("textlabel") as Label;

        blackOverlayElement = base.GetUIElement("blackborder");
        blackOverlayElement.SetVisible(false);
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
        blackOverlayElement.SetVisible(true);
        blackOverlayElement.SetEnabled(true);
        //textlabel.SetEnabled(true);

        /*
        yield return new WaitForSeconds(1);
        textlabel.visible = true;
        textlabel.text = newSceneName;
        yield return new WaitForSeconds(1);
        textlabel.visible = false;
        */
        yield return new WaitForSeconds(0.45f);

        blackOverlayElement.SetEnabled(false);
        blackOverlayElement.SetVisible(false);

        //textlabel.SetEnabled(false);

        UnityEngine.SceneManagement.SceneManager.LoadScene(newSceneName);
    }
}
