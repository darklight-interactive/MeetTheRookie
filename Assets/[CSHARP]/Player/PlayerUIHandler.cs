using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using UnityEngine;

[RequireComponent(typeof(OverlapGrid2D))]
public class PlayerUIHandler : MonoBehaviour
{
    OverlapGrid2D overlapGrid2D => GetComponent<OverlapGrid2D>();

    public void CreateChoices()
    {

    }
}
