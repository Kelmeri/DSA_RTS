using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target; // The target object to follow
    public Vector3 offset;   // Offset from the target object

    private void Start()
    {
        GameObject targetObject = GameObject.Find("Paladin_J_Nordstrom");
        target = targetObject.transform;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Set the camera's position to the target's position plus the offset
            transform.position = target.position + offset;

            // Make the camera look at the target
            transform.LookAt(target);
        }
    }
}

