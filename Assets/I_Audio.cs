using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Audio : MonoBehaviour, IInteractable
{
    string prefix = "((I_AUDIO)) ";

    public void Interact()
    {
        Debug.Log(prefix + " Audio Trigger");
    }
}
