using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UnityExt.UXML
{
    /// <summary>
    /// Not actually a vector field (https://en.wikipedia.org/wiki/Vector_field), but a sloppy interpretation of one.
    /// Basically, given a 2D grid of positions, a present position, and a given direction, where will you go based on the direction?
    /// </summary>
    /// <typeparam name="TElement">The selectables we'll be looking over.</typeparam>
    public class SelectableVectorField<TElement> where TElement : VisualElement, new()
    {
        HashSet<TElement> _selectables = new HashSet<TElement>();
        public HashSet<TElement> Selectables
        {
            get { return _selectables; }
            set { _selectables = value; }
        }
        public TElement PreviousSelection { get; private set; }
        public TElement CurrentSelection { get; private set; }
        public void Add(TElement selectable)
        {
            _selectables.Add(selectable);
            if (CurrentSelection == null)
                CurrentSelection = selectable;
        }
        public void Remove(TElement selectable)
        {
            _selectables.Remove(selectable);
            if (CurrentSelection == selectable)
            {
                CurrentSelection = _selectables.First();
            }
        }
        public void Load(IEnumerable<TElement> selectables)
        {
            _selectables = new HashSet<TElement>(selectables);
            CurrentSelection = _selectables.First();
        }
        public void Clear()
        {
            _selectables.Clear();
            CurrentSelection = null;
        }
        public void AddRange(IEnumerable<TElement> selectables)
        {
            foreach (TElement selectable in selectables)
            {
                _selectables.Add(selectable);
            }
        }


        /// <summary>
        /// Select an element given our currently selected element.
        /// </summary>
        /// <param name="dir">The direction from which to select.</param>
        /// <returns>If we found a successful element, a new pick. Otherwise it's just the previous one.</returns>
        public TElement GetElementInDirection(Vector2 dir)
        {
            if (_selectables.Count == 0) return null;
            if (this.CurrentSelection == null)
            {
                return this.CurrentSelection;
            }

            // If we have a direction, we can try to find the next element.
            if (dir != Vector2.zero)
            {
                // Potentially select a new element.
                TElement pick = raycastEstimate(CurrentSelection.worldBound.center, dir);
                if (pick != null && pick.name != CurrentSelection.name)
                {
                    PreviousSelection = CurrentSelection;
                    CurrentSelection = pick;
                }
            }
            return CurrentSelection;
        }

        #region ---- Raycast Estimation -------- >>
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
        TElement raycastEstimate(Vector2 from, Vector2 direction, int threshhold = 1000)
        {
            float closestDir = -1;
            TElement selected = null;

            // We parameterize our direction vector.
            // Direction = (dx/dt, dy/dt).
            // So direction.y = dy/dt, y = direction.y * t + C_1.
            // And direction.x = dx/dt, x = direction.x * t + C_2.
            // C_1 and C_2 are just from.y and from.x respectively.
            foreach (TElement selectable in _selectables)
            {
                if (selectable != CurrentSelection)
                {
                    Vector2 selectable_center = selectable.worldBound.center;
                    Debug.Log($"Selectable: {selectable.name} Center: {selectable_center}");

                    // The direction ray gives us an equation we can solve for. But first,
                    // Get a dot product to quickly see if we're headed in the right direction:
                    Vector2 to = selectable_center - from;
                    to.Normalize();
                    float dot = Vector3.Dot(to, direction);
                    if (dot > 0.5f)
                    {
                        // Now we construct a function that, given some value t, gives us the distance to the center of the rect.
                        // If you look at the problem on graph paper, you'll see that (For STRAIGHT LINES) the closest distance to any rect is also the closest point to that rect's center.
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
                        float t = (direction.y * (selectable_center.y - from.y)
                            + direction.x * (selectable_center.x - from.x))
                            / (Mathf.Pow(direction.x, 2) + Mathf.Pow(direction.y, 2));
                        // And so we get the x and y from our t-value:
                        Vector2 dirClose = new Vector2(direction.x * t + from.x, direction.y * t + from.y);
                        // And now we just find the distance, and see if it's closer than the other distances we've calculated.
                        float dist = Vector2.Distance(dirClose, selectable_center);

                        // Check if the point is within the rect bounds.
                        Vector2 selectFullPos = selectable.worldBound.position;
                        bool withinRectBounds = dirClose.x >= selectFullPos.x
                            && dirClose.x <= selectFullPos.x + selectable.worldBound.width
                            && dirClose.y >= selectFullPos.y
                            && dirClose.y <= selectFullPos.y + selectable.worldBound.height;

                        // If we're within the rect bounds, && its the closest , select it.
                        float actualElementDist = Vector2.Distance(from, selectable.worldBound.center);
                        if ((dist <= threshhold || withinRectBounds) && (actualElementDist < closestDir || closestDir == -1))
                        {
                            closestDir = actualElementDist;
                            selected = selectable;
                            //Debug.Log($"Selected: {selected.name} Input : {direction}  {actualElementDist} {closestDir}");
                        }
                    }
                }
            }
            return selected;
        }
        #endregion

        /*
                        // Visualization: https://www.desmos.com/calculator/kl2oypib1f
                        var t = (direction.y * (selectable.worldBound.center.y - from.y)
                            + direction.x * (selectable.worldBound.center.x - from.x))
                            / (Mathf.Pow(direction.x, 2) + Mathf.Pow(direction.y, 2));
                        // And so we get the x and y from our t-value:
                        var dirClose = new Vector2(direction.x * t + from.x, direction.y * t + from.y);
                        // And now we just find the distance, and see if it's closer than the other distances we've calculated.
                        var dist = Vector2.Distance(dirClose, selectable.worldBound.center);

                        // Check if the point is within the rect bounds.
                        Vector2 selectFullPos = selectable.worldBound.position;
                        bool withinRectBounds = dirClose.x >= selectFullPos.x
                            && dirClose.x <= selectFullPos.x + selectable.worldBound.width
                            && dirClose.y >= selectFullPos.y
                            && dirClose.y <= selectFullPos.y + selectable.worldBound.height;

                        // If we're within the rect bounds, && its the closest , select it.
                        float actualElementDist = Vector2.Distance(from, selectable.worldBound.center);
                        if ((dist <= threshhold || withinRectBounds) && (actualElementDist < closestDir || closestDir == -1))
                        {
                            closestDir = actualElementDist;
                            selected = selectable;
                            Debug.Log($"Selected: {selected.name} Input : {direction}  {actualElementDist} {closestDir}");
                        }
        */

    }
}
