using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MTRClueContainer : VisualElement
{
    List<MTRClueElement> _clues = new List<MTRClueElement>();

    int _clueCount = 5;
    int _clueSize = 100;

    [UxmlAttribute, Range(1, 10)]
    public int ClueCount
    {
        get => _clueCount;
        set
        {
            _clueCount = value;
            GenerateRandomClues();
        }
    }

    [UxmlAttribute, Range(100, 250)]
    public int ClueSize
    {
        get => _clueSize;
        set
        {
            _clueSize = value;
            GenerateRandomClues();
        }
    }

    public MTRClueContainer()
    {
        this.style.flexGrow = 1;
        this.style.position = Position.Relative;
        this.style.alignSelf = Align.Stretch;
        this.style.flexDirection = FlexDirection.Row;

        GenerateRandomClues();
    }

    /// <summary>
    /// Generates a random set of ClueElements within the container area
    /// </summary>
    private void GenerateRandomClues()
    {
        DestroyAllClues();

        for (int i = 0; i < ClueCount; i++)
        {
            MTRClueElement clue = new MTRClueElement();

            this.Add(clue);
            _clues.Add(clue);
        }
    }

    private void DestroyAllClues()
    {
        foreach (var clue in _clues)
        {
            clue.RemoveFromHierarchy();
        }
        _clues.Clear();
    }
}
