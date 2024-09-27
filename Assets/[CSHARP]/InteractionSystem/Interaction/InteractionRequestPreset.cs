using System.Collections.Generic;
using Darklight.UnityExt.Library;
using UnityEngine;

[CreateAssetMenu(menuName = "Darklight/Interaction/Interactable/RequestPreset")]
public class InteractionRequestDataObject : EnumGameObjectScriptableLibrary<InteractionType>
{



    protected override EnumGameObjectLibrary<InteractionType> CreateNewLibrary()
    {
        return new EnumGameObjectLibrary<InteractionType>()
        {
            ReadOnlyKey = true,
            ReadOnlyValue = false,
        };
    }

    public List<InteractionType> GetKeys()
    {
        return new List<InteractionType>(Keys);
    }
}