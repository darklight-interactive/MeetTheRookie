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


public class TargetInteractionCommand : InteractionCommand<MTRTargetIconReciever>
{
    bool _visible;

    public TargetInteractionCommand(MTRTargetIconReciever reciever, bool visible) : base(reciever)
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

public class DialogueInteractionCommand : InteractionCommand<MTRDialogueReciever>
{
    bool _destroy;
    string _text;
    public DialogueInteractionCommand(MTRDialogueReciever reciever, bool destroy) : base(reciever)
    {
        _destroy = destroy;
    }

    public DialogueInteractionCommand(MTRDialogueReciever reciever, string text) : base(reciever)
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

public class ChoiceInteractionCommand : InteractionCommand<MTRChoiceReciever>
{
    List<Choice> _choices;
    public ChoiceInteractionCommand(MTRChoiceReciever reciever, List<Choice> choices) : base(reciever)
    {
        _choices = choices;
    }

    public override void Execute()
    {
        _reciever.LoadChoices(_choices);
    }
}