using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.UXML;
using UnityEngine;
using NaughtyAttributes;
using Darklight.UnityExt.Editor;
using System.Collections;
using Darklight.UnityExt.Behaviour;




#if UNITY_EDITOR
using UnityEditor;
#endif



public interface IInteractionHandler
{
    InteractionTypeKey TypeKey { get; }
}
