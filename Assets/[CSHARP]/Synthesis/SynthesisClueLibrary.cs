using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class SynthesisClue
{
    [ShowOnly] public string clueName;
    [ShowOnly] public bool isDiscovered;
}

public class SynthesisClueLibrary : ScriptableObject
{
    [SerializeField] private SynthesisClue[] mystery1Clues;

    public void LoadMysteryClues()
    {
        InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
        if (storyObject == null)
        {
            Debug.LogWarning("Story Object is not initialized.");
            return;
        }

        List<string> clues = storyObject.GetVariableByName("Mystery1").ToStringList();
        mystery1Clues = new SynthesisClue[clues.Count];
        foreach (string clue in clues)
        {
            SynthesisClue newClue = new SynthesisClue();
            newClue.clueName = clue;
            mystery1Clues[clues.IndexOf(clue)] = newClue;
            //newClue.clueElement = new VisualTreeAsset("Assets/Resources/Synthesis/SynthesisClue.uxml");
        }
    }
}