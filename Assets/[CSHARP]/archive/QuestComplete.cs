using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestCompleteType { NONE, SEND_TO }

[System.Serializable]
public abstract class QuestComplete : ScriptableObject
{
    public QuestCompleteType type = QuestCompleteType.NONE;
}
