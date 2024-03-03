using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;

    public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}

