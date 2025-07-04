using System;
using Darklight.UnityExt.Editor;
using UnityEngine;

[RequireComponent(typeof(MTRCharacterAnimator), typeof(MTRCharacterInteractable))]
public class MTR_Misra_Controller : NPC_Controller
{
    public override void Start()
    {
        base.Start();

        // ADD MISRA-SPECIFIC STATES
        GrabbedState grabbedState =
            new(stateMachine, NPCState.GRABBED, new object[] { this.stateMachine });
        StruggleState struggleState =
            new(stateMachine, NPCState.STRUGGLE, new object[] { this.stateMachine });
        DraggedState draggedState =
            new(stateMachine, NPCState.DRAGGED, new object[] { this.stateMachine });

        stateMachine.AddState(grabbedState);
        stateMachine.AddState(struggleState);
        stateMachine.AddState(draggedState);

        stateMachine.ClearState();
    }
}
