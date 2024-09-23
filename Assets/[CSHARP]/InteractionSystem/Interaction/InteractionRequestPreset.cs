using System.Collections.Generic;
using Darklight.UnityExt.Library;
using UnityEngine;

[CreateAssetMenu(menuName = "Darklight/Interaction/Interactable/RequestPreset")]
public class InteractionRequestPreset : EnumGameObjectScriptableLibrary<InteractionTypeKey>
{
    [SerializeField]
    EnumObjectLibrary<InteractionTypeKey, GameObject> _recieverPrefabLibrary = new EnumObjectLibrary<InteractionTypeKey, GameObject>()
    {
        ReadOnlyKey = true,
        ReadOnlyValue = false,
        RequiredKeys = new List<InteractionTypeKey>()
        {
            InteractionTypeKey.TARGET
        }
    };

    public GameObject CreateRecieverGameObject(InteractionTypeKey key)
    {
        if (_recieverPrefabLibrary.TryGetValue(key, out GameObject prefab))
        {
            return Instantiate(prefab);
        }

        return null;
    }
}