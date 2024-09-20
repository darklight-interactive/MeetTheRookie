using System.Collections;
using System.Collections.Generic;
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
