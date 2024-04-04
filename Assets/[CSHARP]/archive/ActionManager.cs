using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public SceneManager sceneManager;
    public QuestManager questManager;
    public Inventory inventory;

    /*
    reference to:
    - player,
    - scene manager,
    - quest manager,
    */

    /*
    receive:  {item}
    delete: {item}
    clear inventory: {}
    sendPlayerTo: {location}
    enterMinigame: {minigame}
    enterCutscene: {cutscene}
    */

    public void Awake()
    {
        inventory = GetComponent<Inventory>();
        questManager = GetComponent<QuestManager>();
        sceneManager = GetComponent<SceneManager>();
    }

    public void RecieveItem(Item item){
        inventory.AddToInventory(item);
        questManager.UpdateActiveQuestLines();
    }

    public bool CheckFetchQuest(List<Item> input_items)
    {
        return inventory.AreItemsInInventory(input_items);
    }

    public void ReadQuestComplete(QuestComplete questComplete)
    {
        if (questComplete.type == QuestCompleteType.SEND_TO)
        {
            Location_QuestComplete locationOnComplete = questComplete as Location_QuestComplete;
            Debug.Log("Action Manager: SEND TO " + locationOnComplete.locationType);
        }
    }

}
