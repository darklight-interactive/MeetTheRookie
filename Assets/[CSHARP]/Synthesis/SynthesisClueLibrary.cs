using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;
using static Darklight.UnityExt.Inky.InkyStoryManager;

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
        MTRStoryManager.TryGetVariableContainer("Mystery1", out StoryVariableContainer mystery1);
        List<string> clues = mystery1.ToStringList();

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