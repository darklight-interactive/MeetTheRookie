using System.Collections.Generic;
using Darklight.UnityExt.Library;
using UnityEngine;

[CreateAssetMenu(menuName = "Darklight/Interaction/Interactable/RequestPreset")]
public class InteractionRequestDataObject : EnumGameObjectScriptableLibrary<InteractionType>
{
    [SerializeField]
    EnumGameObjectLibrary<InteractionType> _dataLibrary = new EnumGameObjectLibrary<InteractionType>
    {
        ReadOnlyKey = false,
        ReadOnlyValue = false
    };

    public override EnumGameObjectLibrary<InteractionType> DataLibrary => _dataLibrary;

    public List<InteractionType> GetKeys()
    {
        return new List<InteractionType>(Keys);
    }
}