using Darklight.Game.Grid;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UXML;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerDialogueHandler : OverlapGrid2D
{

}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerDialogueHandler), true)]
public class PlayerDialogueHandlerEditor : OverlapGrid2DEditor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
#endif

