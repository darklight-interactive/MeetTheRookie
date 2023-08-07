using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLine : MonoBehaviour
{
    public string description, title;
    public int ID;

    [Header("Quest Steps")]
    public QuestStep currentQuest;
    public List <QuestStep> quests;


}
