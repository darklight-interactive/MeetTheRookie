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