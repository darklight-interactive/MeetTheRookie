using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Quick and dirty class for dragging synthesis items across a scene.
/// </summary>
public class SynthesisDraggable : PointerManipulator
{
    Rect globalStart;
    /// <summary>
    /// Because we don't want to mess with other items without relativePosition,
    /// we use some hacks to get a position on the first drag.
    /// </summary>
    /// 
    bool globalStartInit = false;
    public SynthesisDraggable(VisualElement target) {
        this.target = target;
    }

    protected override void RegisterCallbacksOnTarget() {
        target.RegisterCallback<PointerDownEvent>(PointerDown);
        target.RegisterCallback<PointerMoveEvent>(PointerMove);
        target.RegisterCallback<PointerUpEvent>(PointerUp);
    }

    protected override void UnregisterCallbacksFromTarget() {
        target.UnregisterCallback<PointerDownEvent>(PointerDown);
        target.UnregisterCallback<PointerMoveEvent>(PointerMove);
        target.UnregisterCallback<PointerUpEvent>(PointerUp);
    }

    bool isDragging = false;
    Vector3 start;
    Vector2 targetStart;


    void PointerDown(PointerDownEvent evt) {
        target.CapturePointer(evt.pointerId);
        start = evt.position;
        targetStart = target.transform.position;
        isDragging = true;
        if (!globalStartInit) {
            globalStart = target.worldBound;
            globalStartInit = true;
        }
    }

    void PointerMove(PointerMoveEvent evt) {
        if (isDragging && target.HasPointerCapture(evt.pointerId)) {
            Vector3 delta = evt.position - start;

            var bounds = target.panel.visualTree.worldBound;
            target.transform.position = new Vector2(
                Mathf.Clamp(targetStart.x + delta.x, -globalStart.x, bounds.width - globalStart.x - globalStart.width),
                Mathf.Clamp(targetStart.y + delta.y, -globalStart.y, bounds.height - globalStart.y - globalStart.height));
        }
    }

    void PointerUp(PointerUpEvent evt) {
        if (isDragging && target.HasPointerCapture(evt.pointerId)) {
            isDragging = false;
            target.ReleasePointer(evt.pointerId);
        }
    }
}
