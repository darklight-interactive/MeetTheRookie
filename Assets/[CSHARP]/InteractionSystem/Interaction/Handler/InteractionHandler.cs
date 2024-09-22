using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.UXML;
using UnityEngine;
using NaughtyAttributes;
using Darklight.UnityExt.Editor;
using System.Collections;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Library;
using System;






#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class InteractionHandler : MonoBehaviour
{
    public abstract void RecieveCommand(IInteractionCommand command);
    public abstract void HandleCommand();
}

