using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLine : MonoBehaviour
{
    [HideInInspector]
    public QuestManager questManager;
    [HideInInspector]
    public ActionManager actionManager;
    public string description, title;
    public int ID;

    [Header("Quest Steps")]
    public QuestStep currentQuest;

    [SerializeReference]
    public List<QuestStep> quests;

    public void Start()
    {
        questManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<QuestManager>();
        actionManager = questManager.actionManager;
        currentQuest = quests[0];
    }

    public void UpdateQuestLine()
    {
        // if Fetch Quest check if items are in inventory
        if (currentQuest.questType == QuestType.FETCH)
        {
            // get ref to child version of currentQuest
            FetchQuestStep fetchQuestRef = currentQuest as FetchQuestStep;

            bool satisfied = actionManager.CheckFetchQuest(fetchQuestRef.fetchItems);
            Debug.Log(fetchQuestRef.name + " SATISFIED: " + satisfied);

            if (satisfied)
            {
                actionManager.ReadQuestComplete(fetchQuestRef.onComplete);
                currentQuest = quests[1];
            }
        }
    }
}
