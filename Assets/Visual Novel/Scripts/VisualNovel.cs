using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using System.Reflection;

public class VisualNovel : UXML_UIDocumentObject
{
    public Vector2 textSize = new Vector2(20, 48);
    
    public TextAsset inkFile;
    
    InputSystemUIInputModule uiInput;
    
    Story currentStory;
    
    VisualElement misraImage;
    VisualElement lupeImage;
    VisualElement continueTriangle;
    VisualElement dialogueBox;
    VisualElement nameTag;
    TextElement dialogueText;
    TextElement nameText;
    VisualElement choiceParent;
    List<Button> choiceButtons = new List<Button>(4);
    
    // Start is called before the first frame update
    void Start()
    {
        uiInput = FindAnyObjectByType<InputSystemUIInputModule>();
        
        misraImage = root.Q<VisualElement>("MisraImage");
        lupeImage  = root.Q<VisualElement>("LupeImage");
        
        continueTriangle = root.Q<VisualElement>("DialogueTriangle");
        dialogueBox = root.Q<VisualElement>("DialogueBox");
        nameTag = root.Q<VisualElement>("NameTag");
        dialogueText = root.Q<TextElement>("DialogueText");
        nameText = root.Q<TextElement>("NameText");
        
        choiceParent = root.Q<VisualElement>("ChoiceParent");
        for(int i = 0; i<choiceButtons.Capacity; i++){
            choiceButtons.Add(root.Q<Button>("Choice"+i));
        }
        choiceButtons[0].clicked += () => SelectChoice0();
        choiceButtons[1].clicked += () => SelectChoice1();
        choiceButtons[2].clicked += () => SelectChoice2();
        choiceButtons[3].clicked += () => SelectChoice3();
        
        currentStory = new Story(inkFile.text);
        
        ContinueStory();
        MoveTriangle();
    }
    
    void ContinueStory(){
        if(currentStory.canContinue){
            UpdateDialogue(currentStory.Continue());
        }
        else if(currentStory.currentChoices.Count>0){
            PopulateChoices();
        }
        else if(currentStory.currentChoices.Count<=0){
            EndStory();
        }
    }
    
    void PopulateChoices(){
        choiceParent.style.display = DisplayStyle.Flex;
        continueTriangle.style.visibility = Visibility.Hidden;
        int index = 0;
        foreach(Choice choice in currentStory.currentChoices){
            choiceButtons[index].style.display = DisplayStyle.Flex;
            choiceButtons[index].text = choice.text;
            index++;
        }
        
        for(int i = index; i<choiceButtons.Count; i++){
            choiceButtons[i].style.display = DisplayStyle.None;
        }
        nameTag.AddToClassList("NameTagLupe");
        nameTag.RemoveFromClassList("NameTagMisra");
        lupeImage.RemoveFromClassList("Inactive");
        misraImage.AddToClassList("Inactive");
    }
    
    void SelectChoice0(){SelectChoice(0);}
    void SelectChoice1(){SelectChoice(1);}
    void SelectChoice2(){SelectChoice(2);}
    void SelectChoice3(){SelectChoice(3);}
    
    void SelectChoice(int index){
        Debug.Log(index);
        // Debug.Log(currentStory.currentChoices.Count);
        // Debug.Log(currentStory.currentChoices[index]);
        currentStory.ChooseChoiceIndex(index);
        choiceParent.style.display = DisplayStyle.None;
        continueTriangle.style.visibility = Visibility.Visible;
        ContinueStory();
    }
    
    void EndStory(){
        UpdateDialogue("END OF STORY");
        Debug.Log("END OF STORY");
    }
    
    void UpdateDialogue(string dialogue){
        List<string> tags = currentStory.currentTags;
        if(tags.Count > 0){
            foreach(string tag in tags){
                string[] splitTag = tag.Split(":");
                if(splitTag[0].Trim()=="name"){
                    nameTag.style.visibility = Visibility.Visible;
                    string name = splitTag[1].Trim();
                    if(name == "Lupe"){
                        nameTag.AddToClassList("NameTagLupe");
                        nameTag.RemoveFromClassList("NameTagMisra");
                        lupeImage.RemoveFromClassList("Inactive");
                        misraImage.AddToClassList("Inactive");
                    }
                    else if(name == "Misra"){
                        misraImage.style.visibility = Visibility.Visible;
                        nameTag.AddToClassList("NameTagMisra");
                        nameTag.RemoveFromClassList("NameTagLupe");
                        misraImage.RemoveFromClassList("Inactive");
                        lupeImage.AddToClassList("Inactive");
                    }
                    break;
                }
                else{
                    nameTag.style.visibility = Visibility.Hidden;
                    // nameText.text = "";
                }
            }
        }
        else{
            nameTag.style.visibility = Visibility.Hidden;
            // nameText.text = "";
        }
        dialogueText.text = dialogue;
        UpdateBoxNTextSize();
    }
    
    void UpdateBoxNTextSize(){
        Vector2 newBoxSize = dialogueText.MeasureTextSize(dialogueText.text, 1000, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
        dialogueBox.style.height = newBoxSize.y;
        
        dialogueText.style.fontSize = Mathf.Min(Mathf.Max(textSize.y*(textSize.y*7/newBoxSize.y), textSize.x), textSize.y);
    }
    
    void MoveTriangle(){
        continueTriangle.ToggleInClassList("TriangleDown");
        continueTriangle.RegisterCallback<TransitionEndEvent>(evt => continueTriangle.ToggleInClassList("TriangleDown"));
        root.schedule.Execute(() => continueTriangle.ToggleInClassList("TriangleDown")).StartingIn(100);
    }

    // Update is called once per frame
    void Update()
    {
        if(uiInput.submit.action.triggered){
            ContinueStory();
        }
    }
}
