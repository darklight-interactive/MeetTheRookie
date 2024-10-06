using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

public interface IInteractionCommand
{
    void Execute();
}

public abstract class InteractionCommand<IReciever> : IInteractionCommand
    where IReciever : InteractionReciever
{
    protected IReciever _reciever;
    public InteractionCommand(IReciever reciever)
    {
        _reciever = reciever;
    }
    public abstract void Execute();
}


public class TargetInteractionCommand : InteractionCommand<TargetInteractionReciever>
{
    bool _visible;

    public TargetInteractionCommand(TargetInteractionReciever reciever, bool visible) : base(reciever)
    {
        _visible = visible;
    }

    public override void Execute()
    {
        if (_visible)
            _reciever.ShowInteractIcon();
        else
            _reciever.HideInteractIcon();
    }
}

public class DialogueInteractionCommand : InteractionCommand<DialogueInteractionReciever>
{
    bool _destroy;
    string _text;
    public DialogueInteractionCommand(DialogueInteractionReciever reciever, bool destroy) : base(reciever)
    {
        _destroy = destroy;
    }

    public DialogueInteractionCommand(DialogueInteractionReciever reciever, string text) : base(reciever)
    {
        _text = text;
    }

    public override void Execute()
    {
        if (_destroy)
            _reciever.DestroySpeechBubble();
        else
            _reciever.CreateNewSpeechBubble(_text);
    }
}

public class ChoiceInteractionCommand : InteractionCommand<ChoiceInteractionReciever>
{
    List<Choice> _choices;
    public ChoiceInteractionCommand(ChoiceInteractionReciever reciever, List<Choice> choices) : base(reciever)
    {
        _choices = choices;
    }

    public override void Execute()
    {
        _reciever.LoadChoices(_choices);
    }
}