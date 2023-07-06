using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public GameObject interactIcon;
    public float playerSpeed=5f;
    private Vector3 target;
    public Vector2 interactableBoxRange= new Vector2(.1f,1f);
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
        target = transform.position;
        interactIcon.SetActive(false);
        inventoryParent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(!ignoringInputs){
            InputHandler();
            if(Input.GetKeyDown(KeyCode.E)){
                CheckInteraction();
            }
        }

        
    }
    
    #region <<INTERACTIONS>>>
    public void OpenInteractableIcon(){
        interactIcon.SetActive(true);
    }
    public void CloseInteractableIcon(){
        interactIcon.SetActive(false);
    }

    private void CheckInteraction(){
        //box casting, will put a box around our object
        RaycastHit2D[]hits = Physics2D.BoxCastAll(transform.position, interactableBoxRange, 0 , Vector2.zero);
        if(hits.Length>0){
            foreach(RaycastHit2D rc in hits){
                if(rc.transform.TryGetComponent(out IInteractable interactableObj)){
                    interactableObj.Interact();
                    return;
                }
            }
        }
    }
    #endregion
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
            HandleMcMovement();
            if(Input.GetKeyDown(KeyCode.E)){
                CheckInteraction();
            }
        }

    }
    #endregion
    #region <<MOVEMENT>>
    public void HandleMcMovement(){
        
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
