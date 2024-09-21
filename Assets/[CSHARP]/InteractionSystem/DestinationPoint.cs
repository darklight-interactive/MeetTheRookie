using Darklight.UnityExt.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DestinationPoint : MonoBehaviour
{
    public GameObject trackedEntity;
    [NonSerialized] public float x;
    [ShowOnly] float distanceRequired = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        x = gameObject.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isEntityInRange()
    {
        if (trackedEntity == null)
        {
            return false;
        }

        float distance = gameObject.transform.position.x - trackedEntity.transform.position.x;

        if (Mathf.Abs(distance) > distanceRequired)
        {
            return false;
        } else
        {
            return true;
        }
    }
}
