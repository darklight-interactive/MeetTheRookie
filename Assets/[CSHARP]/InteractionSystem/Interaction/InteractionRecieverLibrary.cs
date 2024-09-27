
using System.Collections.Generic;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using UnityEngine;

[System.Serializable]
public class InteractionRecieverLibrary : EnumComponentLibrary<InteractionType, InteractionReciever>
{
    public InteractionRecieverLibrary()
    {
        ReadOnlyKey = true;
        ReadOnlyValue = true;
        RequiredKeys = new InteractionType[] { };
        this.Refresh();
    }

    protected override void InternalClear()
    {
        //Debug.Log("InteractionRecieverLibrary: InternalClear called.");
        base.InternalClear();
    }
}