using System;
using Darklight.UnityExt.Library;

[Serializable]
public class InteractionHandlerLibrary : EnumObjectLibrary<InteractionTypeKey, InteractionHandler>
{
    public InteractionHandlerLibrary() : base(false, false, false)
    {
    }
}