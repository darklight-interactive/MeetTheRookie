using System.Collections;
using System.Collections.Generic;
using Darklight;
using UnityEngine;

public class InkyStoryStateMachine : StateMachine<MainStoryState>
{
    public InkyStoryStateMachine(MainStoryState initialState) : base(initialState) { }
}
