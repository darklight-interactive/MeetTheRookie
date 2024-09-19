using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Darklight/FMODExt/SFXObject")]
public class FMODExt_SFXObject : ScriptableObject
{
    [Header("General Interaction SFX")]
    public EventReference startInteraction;
    public EventReference continuedInteraction;
    public EventReference endInteraction;

    [Header("Menu Interaction SFX")]
    public EventReference menuHover;
    public EventReference menuSelect;

    [Header("Game Interaction SFX")]
    public EventReference itemPickup;
    public EventReference itemDrop;
    public EventReference itemUse;

}