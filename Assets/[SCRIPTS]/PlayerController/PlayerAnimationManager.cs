using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimationManager : MonoBehaviour
{
    public Transform spriteTransform;
    public PlayerController playerController => GetComponentInParent<PlayerController>();

    private int _flipMultiplier = 1;
    public void FlipTransform(Vector2 moveInput)
    {
        if (moveInput.x < 0) { _flipMultiplier = 0; }
        else if (moveInput.x > 0) { _flipMultiplier = 1; }

        spriteTransform.localRotation = Quaternion.Euler(new Vector3(0, 180 * _flipMultiplier, 0));
    }
}
