using UnityEngine;
using FMODUnity;
using System.Collections;


public class PlayerSoundController : MonoBehaviour
{
    [Header("Movement")]
    public EventReference footstepEvent;

    [Header("Interaction")]
    public EventReference interactionEvent;

    [Header("Speech")]
    public EventReference speechEvent;

    private void Awake()
    {

    }



}