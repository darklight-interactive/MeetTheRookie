using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    [HideInInspector]
    public Player_Interaction interaction;

    public float playerSpeed=5f;
    private Vector3 target;
    public bool arrowKeys = true;
    public bool inventoryOpen = false;
    //public Inventory inventory;
    public GameObject inventoryParent;
    
    public KeyCode inventoryKey = KeyCode.I;
    // Start is called before the first frame update

    public GameObject floor;
    public bool ignoringInputs;
    void Start()
    {
        interaction = GetComponentInChildren<Player_Interaction>();

        target = transform.position;
        inventoryParent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!ignoringInputs){
            InputHandler();
        }
    }
    

    #region <<INPUT>>
    public void InputHandler(){
        if (Input.GetKeyDown(inventoryKey))
        {
            inventoryOpen = !inventoryOpen;
            inventoryParent.SetActive(inventoryOpen);
            Debug.Log("SETTING INVENTORY PARENT TO: " + inventoryOpen);
            //set inventory active
        }

        if(!inventoryOpen){
            HandleMovement();
            if(Input.GetKeyDown(KeyCode.E)){
                interaction.InteractWithObject();
            }
        }
    }


    #endregion
    #region <<MOVEMENT>>
    public void HandleMovement(){
        
        if(arrowKeys){
            if(Input.GetKey(KeyCode.RightArrow)){
                target.x+=playerSpeed*Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target, playerSpeed*Time.deltaTime);
            }
            if(Input.GetKey(KeyCode.LeftArrow)){
                target.x-=playerSpeed*Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target, playerSpeed*Time.deltaTime);
            }
        }else{
            if(Input.GetMouseButton(0)){
            target.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            }
            transform.position = Vector3.MoveTowards(transform.position, target, playerSpeed*Time.deltaTime);
        }
    }

    public void MoveUpLadder(Vector3 position){
        ignoringInputs = true;
        //LOGIC FOR ANIMATION HERE
        transform.position = position;
        target.y = transform.position.y;
        ignoringInputs = false;
    }
    public void MoveDownLadder(Vector3 position){
        ignoringInputs = true;
        //LOGIC FOR ANIMATION HERE
        transform.position = position;
        target.y = transform.position.y;
        ignoringInputs = false;
    }
    #endregion

}
