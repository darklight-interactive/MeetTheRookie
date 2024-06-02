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

    [SerializeField] private SynthesisClue[] clues;

}