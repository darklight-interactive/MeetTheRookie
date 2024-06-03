using System.Collections.Generic;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class SynthesisClue
{
    public string clueName;
    public string clueDescription;
    public VisualTreeAsset clueElement;
}

public class SynthesisClueLibrary : ScriptableObject
{

    [Button]
    public void LoadMysteryClues()
    {
        InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
        if (storyObject == null)
        {
            Debug.LogWarning("Story Object is not initialized.");
            return;
        }

        List<string> clues = storyObject.GetVariableByName("Level3_Clues").ToStringList();
        mystery1Clues = new SynthesisClue[clues.Count];
        foreach (string clue in clues)
        {
            SynthesisClue newClue = new SynthesisClue();
            newClue.clueName = clue;
            mystery1Clues[clues.IndexOf(clue)] = newClue;
            //newClue.clueElement = new VisualTreeAsset("Assets/Resources/Synthesis/SynthesisClue.uxml");
        }
    }

    [SerializeField] private SynthesisClue[] mystery1Clues;


}