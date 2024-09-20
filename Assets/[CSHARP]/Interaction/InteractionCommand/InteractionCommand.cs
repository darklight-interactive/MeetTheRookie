using UnityEngine;

public interface IInteractionCommand
{
    InteractionType Type { get; }
    void Execute();
}

public enum InteractionType
{
    SIMPLE, TARGET, DIALOGUE, CHOICE
}

public class InteractionCommand : IInteractionCommand
{
    public InteractionType Type => InteractionType.SIMPLE;

    public void Execute()
    {
        throw new System.NotImplementedException();
    }
}