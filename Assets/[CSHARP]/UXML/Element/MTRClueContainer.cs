using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MTRClueContainer : VisualElement
{
    MTRMysteryDataSO _mysteryData;
    List<MTRClueElement> _clues = new List<MTRClueElement>();
    VisualElement _headerElement;
    Label _headerTitle;
    VisualElement _clueContainerElement;
    bool _discoverAllOverride = false;

    [UxmlAttribute]
    public MTRMysteryDataSO MysteryData
    {
        get => _mysteryData;
        set
        {
            _mysteryData = value;
            GenerateClues();
        }
    }

    [UxmlAttribute]
    public bool DiscoverAllOverride
    {
        get => _discoverAllOverride;
        set
        {
            _discoverAllOverride = value;
            foreach (var clue in _clues)
            {
                clue.IsDiscovered = value;
            }
        }
    }

    public List<MTRClueElement> ClueList => _clues;

    public MTRClueContainer()
    {
        this.style.flexGrow = 1;
        this.style.position = Position.Relative;
        this.style.alignSelf = Align.Stretch;
        this.style.flexDirection = FlexDirection.Column;

        CreateHeader();
        CreateClueContainer();
        GenerateClues();
    }

    void CreateHeader()
    {
        _headerElement = new VisualElement()
        {
            name = "header",
            style =
            {
                position = Position.Relative,
                alignSelf = Align.Stretch,
                justifyContent = Justify.Center,
                alignItems = Align.Center,
                backgroundColor = new Color(0, 0, 0, 0.5f),
                width = Length.Percent(100),
            }
        };
        this.Add(_headerElement);

        _headerTitle = new Label()
        {
            text = "Mystery Title",
            style =
            {
                color = Color.white,
                flexGrow = 1,
                alignSelf = Align.Center,
            }
        };
        _headerElement.Add(_headerTitle);
    }

    void SetHeaderTitle(string title)
    {
        _headerTitle.text = title;
    }

    void CreateClueContainer()
    {
        _clueContainerElement = new GroupBox() { name = "clue-container" };
        this.Add(_clueContainerElement);
    }

    void GenerateClues()
    {
        DestroyAllClues();
        if (_mysteryData == null)
            return;

        SetHeaderTitle(_mysteryData.mysteryName);
        foreach (MTRInteractableDataSO clue in _mysteryData.clueDataList)
        {
            MTRClueElement clueElement = new MTRClueElement(
                clue,
                _mysteryData.GetRandomClueBackground()
            );
            _clues.Add(clueElement);
            _clueContainerElement.Add(clueElement);
        }
    }

    void DestroyAllClues()
    {
        foreach (var clue in _clues)
        {
            clue.RemoveFromHierarchy();
        }
        _clues.Clear();
    }

    /// <summary>
    /// Returns true if the clue container data contains the given clue tag.
    /// </summary>
    /// <param name="clueTag">The tag of the clue to check for.</param>
    /// <param name="clueElement">The clue element if found, otherwise null.</param>
    /// <returns>True if the clue container data contains the given clue tag, otherwise false.</returns>
    public bool Contains(string clueTag, out MTRClueElement clueElement)
    {
        foreach (MTRClueElement clue in _clues)
        {
            if (clue.InteractableData.ClueTag == clueTag)
            {
                clueElement = clue;
                return true;
            }
        }
        clueElement = null;
        return false;
    }

    /// <summary>
    /// Returns true if all clues in the container are discovered.
    /// </summary>
    /// <returns>True if all clues are discovered, otherwise false.</returns>
    public bool IsComplete()
    {
        foreach (var clue in _clues)
        {
            if (!clue.IsDiscovered)
                return false;
        }
        return true;
    }
}
