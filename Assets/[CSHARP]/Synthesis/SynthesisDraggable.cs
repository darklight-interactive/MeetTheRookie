using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SynthesisDraggable : PointerManipulator
{
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
    Rect globalStart;


    void PointerDown(PointerDownEvent evt) {
        target.CapturePointer(evt.pointerId);
        start = evt.position;
        targetStart = target.transform.position;
        globalStart = target.worldBound;
        isDragging = true;
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
