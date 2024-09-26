
using Darklight.UnityExt.Library;

[System.Serializable]
public class InteractionRecieverLibrary : EnumComponentLibrary<InteractionType, InteractionReciever>
{
    public InteractionRecieverLibrary()
    {
        ReadOnlyKey = true;
        ReadOnlyValue = true;
        RequiredKeys = new InteractionType[]
        {
            InteractionType.TARGET
        };
        this.Refresh();
    }
}