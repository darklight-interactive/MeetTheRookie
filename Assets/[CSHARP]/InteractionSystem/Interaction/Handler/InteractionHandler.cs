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

public abstract class BaseInteractionHandler : MonoBehaviour
{
    public abstract InteractionTypeKey TypeKey { get; }
    public abstract void RecieveCommand(IInteractionCommand command);
    public abstract void HandleCommand();
}

[Serializable]
public class InteractionHandler : BaseInteractionHandler
{
    [SerializeField] Interactable _interactable;
    public override InteractionTypeKey TypeKey => InteractionTypeKey.SIMPLE;
    IInteractionCommand _command;

    public void AttachInteractable(Interactable interactable)
    {
        _interactable = interactable;
    }

    public override void RecieveCommand(IInteractionCommand command)
    {
        if (command.InteractionType == TypeKey)
            _command = command;
    }
    public override void HandleCommand()
    {
        _command.Execute();
    }
}

