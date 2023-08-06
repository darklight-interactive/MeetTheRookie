using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TestQuestStep {
    public string name;
    public int id;

    public QuestStep step_object;

    public UnityEvent testEvent;
}

public class TestQuestLine : MonoBehaviour
{

    [SerializeField]
    public List<TestQuestStep> steps = new();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
