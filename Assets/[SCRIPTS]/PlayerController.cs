using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Input;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInteraction))]
public class PlayerController : MonoBehaviour
{
    UniversalInputManager inputManager => UniversalInputManager.Instance;
    PlayerInteraction playerInteraction => GetComponent<PlayerInteraction>();
    [SerializeField] private Vector2 activeMoveInput = Vector2.zero;

    // =============== [ PUBLIC INSPECTOR VALUES ] =================== //
    [Range(0.1f, 1f)] public float playerSpeed = 5f;
    public GameObject floor;
    public bool ignoringInputs;
    void Start()
    {
        Invoke("StartInputListener", 1);

        //target = transform.position;
        //inventoryParent.SetActive(false);
    }

    void StartInputListener()
    {
        // Subscribe to Universal MoveInput
        InputAction moveInputAction = UniversalInputManager.MoveInputAction;
        moveInputAction.performed += context => activeMoveInput = moveInputAction.ReadValue<Vector2>();
        moveInputAction.canceled += context => activeMoveInput = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ignoringInputs)
        {
            HandleMovement();
        }
    }


    #region <<INPUT>>
    public void InputHandler()
    {
        /*
        if (Input.GetKeyDown(inventoryKey))
        {
            inventoryOpen = !inventoryOpen;
            inventoryParent.SetActive(inventoryOpen);
            Debug.Log("SETTING INVENTORY PARENT TO: " + inventoryOpen);
            //set inventory active
        }

        if (!inventoryOpen)
        {
            HandleMovement();
            if (Input.GetKeyDown(KeyCode.E))
            {
                interaction.InteractWithObject();
            }
        }*/
    }
    #endregion

    #region <<MOVEMENT>>
    public void HandleMovement()
    {
        Vector2 moveDirection = activeMoveInput; // Get the base Vec2 Input value
        moveDirection *= playerSpeed; // Scalar
        moveDirection *= Vector2.right; // Nullify the Y axis

        Vector3 targetPosition = transform.position + (Vector3)moveDirection;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);

        /*
        if (arrowKeys)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                target.x += playerSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target, playerSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                target.x -= playerSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target, playerSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                target.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            }
            transform.position = Vector3.MoveTowards(transform.position, target, playerSpeed * Time.deltaTime);
        }*/

    }

    public void MoveUpLadder(Vector3 position)
    {
        ignoringInputs = true;
        //LOGIC FOR ANIMATION HERE
        transform.position = position;
        //target.y = transform.position.y;
        ignoringInputs = false;
    }
    public void MoveDownLadder(Vector3 position)
    {
        ignoringInputs = true;
        //LOGIC FOR ANIMATION HERE
        transform.position = position;
        //target.y = transform.position.y;
        ignoringInputs = false;
    }
    #endregion

}


