using System;
using UnityEngine;

[Serializable]
public class SimpleInteractionHandler : InteractionHandler
{
    IInteractionCommand _command;

    public override InteractionTypeKey TypeKey => InteractionTypeKey.SIMPLE;
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