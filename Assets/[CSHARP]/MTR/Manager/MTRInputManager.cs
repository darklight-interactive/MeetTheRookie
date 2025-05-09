using UnityEngine;
using Darklight.UnityExt.Input;
using UnityEngine.InputSystem;
using System;
public class MTRInputManager : UniversalInputManager
{
    public new static MTRInputManager Instance;

    //InputAction _tertiaryInteract => activeActionMap.FindAction("TertiaryInteract");

    public static event Action OnTertiaryInteract;
}