using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Input;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInteraction), typeof(PlayerAnimationManager))]
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

    public void HandleMovement()
    {
        Vector2 moveDirection = activeMoveInput; // Get the base Vec2 Input value
        moveDirection *= playerSpeed; // Scalar
        moveDirection *= Vector2.right; // Nullify the Y axis

        // Set Target Position & Apply
        Vector3 targetPosition = transform.position + (Vector3)moveDirection;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);


        // Update the Animation
        PlayerAnimationManager animationManager = GetComponent<PlayerAnimationManager>();
        animationManager.FlipTransform(activeMoveInput);
    }
}


