using UnityEngine;

public interface IInteractionCommand
{
    InteractionTypeKey Type { get; }
    void Execute();
}

public class InteractionCommand : IInteractionCommand
{
    public InteractionTypeKey Type => InteractionTypeKey.SIMPLE;

    public void Execute()
    {
        throw new System.NotImplementedException();
    }
}