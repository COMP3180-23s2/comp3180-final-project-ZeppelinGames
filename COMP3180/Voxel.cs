using System;
using UnityEngine;

public class Voxel
{
    public Vector3Int position;

    public Voxel(Vector3Int position)
    {
        this.position = position;
    }

    public Voxel(Vector3 position)
    {
        this.position = new Vector3Int(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.y),
            Mathf.RoundToInt(position.z));
    }
}
