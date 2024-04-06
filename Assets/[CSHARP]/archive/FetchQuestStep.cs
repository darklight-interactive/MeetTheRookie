using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFetchQuestStep", menuName = "QuestStep/FetchQuest")]
[System.Serializable]
public class FetchQuestStep : QuestStep
{
    // Default constructor to set default values
    public FetchQuestStep()
    {
        questType = QuestType.FETCH;
        title = "Fetch Quest";
        ID = 1001;
    }

    public List<Item> fetchItems = new();
}