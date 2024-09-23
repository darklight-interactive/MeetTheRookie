using System.Collections.Generic;
using Darklight.UnityExt.Library;
using UnityEngine;

[CreateAssetMenu(menuName = "Darklight/Interaction/Interactable/RequestPreset")]
public class InteractionRequestPreset : EnumGameObjectScriptableLibrary<InteractionTypeKey>
{
    protected override EnumGameObjectLibrary<InteractionTypeKey> CreateNewLibrary()
    {
        return new EnumGameObjectLibrary<InteractionTypeKey>()
        {
            ReadOnlyKey = true,
            ReadOnlyValue = false,
        };
    }

    public GameObject CreateRecieverGameObject(InteractionTypeKey key)
    {
        TryGetValue(key, out GameObject go);
        if (go != null)
        {
            return Instantiate(go);
        }
        return null;
    }
}