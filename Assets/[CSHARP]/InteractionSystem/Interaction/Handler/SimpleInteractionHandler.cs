using UnityEngine;

public class SimpleInteractionHandler : InteractionHandler
{
    IInteractionCommand _command;

    public InteractionTypeKey InteractionType => InteractionTypeKey.SIMPLE;
    public IInteractionCommand Command { get; private set; }

    public override void RecieveCommand(IInteractionCommand command)
    {
        if (command.InteractionType == InteractionType)
            _command = command;
    }

    public override void HandleCommand()
    {
        _command.Execute();
    }
}