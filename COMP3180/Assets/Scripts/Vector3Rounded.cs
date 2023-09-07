using System;
using UnityEngine;

public class Vector3Rounded
{
    public float x, y, z, round;
    public Vector3 v3 => new Vector3(this.x, this.y, this.z);

    public Vector3Rounded(float x, float y, float z, float round)
    {
        this.x = x * round;
        this.y = y * round;
        this.z = z * round;
        this.round = round;
    }

    public Vector3Rounded(Vector3 vec, float round)
    {
        this.x = vec.x * round;
        this.y = vec.y * round;
        this.z = vec.z * round;
        this.round = round;
    }

    public static Vector3Rounded operator +(Vector3Rounded a, Vector3Rounded b)
    {
        return new Vector3Rounded(a.x + b.x, a.y + b.y, a.z + b.z, a.round);
    }

    public static Vector3 operator +(Vector3Rounded vr, Vector3 v)
    {
        return new Vector3(vr.x + v.x, vr.y + v.y, vr.z + v.z);
    }
}
