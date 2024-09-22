using UnityEngine;

public interface IInteractionCommand
{
    InteractionTypeKey InteractionType { get; }
    void Execute();
}

public abstract class BaseInteractionCommand : IInteractionCommand
{
    public abstract InteractionTypeKey InteractionType { get; }

    public BaseInteractionCommand() { }
    public void Execute() { }
}
