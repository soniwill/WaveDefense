using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionEnum
{
    Left,
    Right,
    Up,
    Down
}

public class SetOnTriggerDir : MonoBehaviour
{

    public DirectionEnum direction;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        var rb = other.attachedRigidbody;

        var dir = GetDirectionVector();
        rb.velocity = dir;
    }

    public Vector3 GetDirectionVector()
    {
        var dir = Vector3.zero;
        switch (direction)
        {
            case DirectionEnum.Left:
                dir = Vector3.left;
                break;
            case DirectionEnum.Right:
                dir = Vector3.right;
                break;
            case DirectionEnum.Up:
                dir = Vector3.forward;
                break;
            case DirectionEnum.Down:
                dir = Vector3.back;
                break;
        }

        return dir;
    }
}
