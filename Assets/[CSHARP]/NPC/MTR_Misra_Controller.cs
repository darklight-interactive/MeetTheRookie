using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using System;

[RequireComponent(typeof(NPC_Animator), typeof(MTRCharacterInteractable))]

public class MTR_Misra_Controller : NPC_Controller {
    public override void Start()
    {
        base.Start();

        // ADD MISRA-SPECIFIC STATES
        GrabbedState grabbedState = new(stateMachine, NPCState.GRABBED, new object[] { this.stateMachine });
        StruggleState struggleState = new(stateMachine, NPCState.STRUGGLE, new object[] { this.stateMachine });
        DraggedState draggedState = new(stateMachine, NPCState.DRAGGED, new object[] { this.stateMachine });

        stateMachine.AddState(grabbedState);
        stateMachine.AddState(struggleState);
        stateMachine.AddState(draggedState);

        stateMachine.ClearState();
    }
}