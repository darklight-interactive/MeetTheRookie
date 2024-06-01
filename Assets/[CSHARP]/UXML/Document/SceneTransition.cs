using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.UXML;

public class SceneTransition : UXML_UIDocumentObject
{
    VisualElement blackOverlayElement;
    Label textlabel;
    public bool isStarted;
    public bool isComplete;

    // Start is called before the first frame update
    public override void Initialize(UXML_UIDocumentPreset preset, string[] tags = null)
    {
        // Initialize the base class
        base.Initialize(preset, tags);

        textlabel = ElementQuery<Label>();

        blackOverlayElement = ElementQuery<VisualElement>("blackOverlay");
        blackOverlayElement.visible = false;
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
        blackOverlayElement.visible = true;

        //textlabel.SetEnabled(true);

        /*
        yield return new WaitForSeconds(1);
        textlabel.visible = true;
        textlabel.text = newSceneName;
        yield return new WaitForSeconds(1);
        textlabel.visible = false;
        */
        yield return new WaitForSeconds(0.45f);

        blackOverlayElement.visible = true;

        //textlabel.SetEnabled(false);

        UnityEngine.SceneManagement.SceneManager.LoadScene(newSceneName);
    }
}
