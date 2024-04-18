using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Not actually a vector field (https://en.wikipedia.org/wiki/Vector_field), but a sloppy interpretation of one.
/// Basically, given a 2D grid of positions, a present position, and a given direction, where will you go based on the direction?
/// </summary>
/// <typeparam name="T">The selectables we'll be looking over.</typeparam>
public class SelectableVectorField<T> where T: VisualElement 
{
    /// <summary>
    /// A list of selectable UI elements.
    /// </summary>
    List<T> selectables;
    public int numSelectables { get {  return selectables.Count; } }

    public T currentlySelected = null;
    // TODO: Make this work with dynamically added elements.
    public SelectableVectorField() {
        this.selectables = new List<T>();
    }

    /// <summary>
    /// Like a raycast, except this also accounts for rays that are CLOSE enough for the MenuVector field to qualify as "good enough".
    /// So that means that this raycast returns some kind of Selectable, provided that Selectable is:
    /// Hit by the ray(the ray at some point is within its rectangle).
    /// This will return the selectable that has the closest distance to where the raycast "hits",
    /// provided the distance is within the "threshold".
    /// We prioritize elements that are closer to the current element, since this is meant to be used for navigation.
    /// </summary>
    /// <param name="from">Position in space.</param>
    /// <param name="direction">Any direction relative to the position.</param>
    /// <param name="threshhold">The maximum distance a given Selectable from `from` can have to be selected.</param>
    /// <returns>Index of closest selectable from raycast.</returns>
    T raycastEstimate(Vector2 from, Vector2 direction, int threshhold = 100) {
        float closestDir = -1;
        T selected = null;

        // We parameterize our direction vector.
        // Direction = (dx/dt, dy/dt).
        // So direction.y = dy/dt, y = direction.y * t + C_1.
        // And direction.x = dx/dt, x = direction.x * t + C_2.
        // C_1 and C_2 are just from.y and from.x respectively.
        foreach(var selectable in selectables) {
            if (selectable != currentlySelected) {
                // The direction ray gives us an equation we can solve for. But first,

                // Get a dot product to quickly see if we're headed in the right direction:
                var to = selectable.worldBound.center - from;
                to.Normalize();
                var dot = Vector3.Dot(to, direction);
                if (dot > 0.5f) {
                    // Now we construct a function that, given some value t, gives us the distance to the center of the rect.
                    // If you look at the problem on graph paper, you'll see that (For STRAIGHT LINES) the closest distance to
                    // any rect is also the closest point to that rect's center.
                    // Then we want to find the global minima of that function.

                    // So, we construct the distance function between any given point on our line, and the center of our rectangle:
                    // sqrt( (direction.y * t + C_1 - center.y, 2)^2 + (direction.x * t + C_2 - center.x)^2 )
                    // Then we optimize to find the global minima.
                    // Taking the derivative with respect to t, we get
                    // (direction.y * (direction.y * t + C_1 - center.y) + direction.x * (direction.x * t + C_2 - center.x))/the whole sqrt from before,I'm not gonna write it all out.
                    // We want the smallest value, so we need to find where the values of dt are zero. Setting the LHS to zero, we get:
                    // 0 = direction.y * (direction.y * t + C_1 - center.y) + direction.x * (direction.x * t + C_2 - center.x).
                    // And there's only one value of t, which is our global minima:
                    // t = (direction.y * (center.y - C_1) + direction.x * (center.x - C_2))/(direction.x^2 + direction.y^2).

                    // Visualization: https://www.desmos.com/calculator/kl2oypib1f

                    var t = (direction.y * (selectable.worldBound.center.y - from.y)
                        + direction.x * (selectable.worldBound.center.x - from.x)) 
                        / (Mathf.Pow(direction.x, 2) + Mathf.Pow(direction.y, 2));
                    // And so we get the x and y from our t-value:
                    var dirClose = new Vector2(direction.x * t + from.x, direction.y * t + from.y);
                    // And now we just find the distance, and see if it's closer than the other distances we've calculated.
                    var dist = Vector2.Distance(dirClose, selectable.worldBound.center);

                    var selectFullPos = selectable.worldBound.position;
                    var withinRectBounds = dirClose.x >= selectFullPos.x
                        && dirClose.x <= selectFullPos.x + selectable.worldBound.width
                        && dirClose.y >= selectFullPos.y
                        && dirClose.y <= selectFullPos.y + selectable.worldBound.height;

                    var actualElementDist = Vector2.Distance(from, selectable.worldBound.center);
                    if ((dist <= threshhold || withinRectBounds) && (actualElementDist < closestDir || closestDir == -1)) {
                        closestDir = actualElementDist;
                        selected = selectable;
                    }
                }
            }
        }
        return selected;
    }

    public void Add(T selectable) {
        selectables.Add(selectable);
        if (this.currentlySelected == null) {
            this.currentlySelected = selectable;
        }
    }

    public void Remove(T selectable) {
        selectables.Remove(selectable);
        if (selectables.Count == 0) {
            this.currentlySelected = null;
        }
    }

    /// <summary>
    /// Select an element given our currently selected element.
    /// </summary>
    /// <param name="dir">The direction from which to select.</param>
    /// <returns>If we found a successful element, a new pick. Otherwise it's just the previous one.</returns>
    public T getFromDir(Vector2 dir) {
        if (selectables.Count == 0) return null;

        if (this.currentlySelected == null) {
            return selectables[0];
        }

        if (dir != Vector2.zero) {
            T pick = raycastEstimate(currentlySelected.worldBound.center, dir);

            if (pick != null) {
                currentlySelected = pick;
            }
        }
        return currentlySelected;
    }
}
