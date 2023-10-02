using UnityEngine;

public struct VoxelPoint
{
    public Vector3Int Position;
    public int ColorIndex;

    public Vector3 WorldPosition => (Vector3)this.Position * VoxelBuilder.VoxelSize;

    public VoxelPoint(Vector3Int v, int cIn)
    {
        this.Position = v;
        this.ColorIndex = cIn;
    }

    public VoxelPoint(int x, int y, int z, int cIn)
    {
        this.Position = new Vector3Int(x, y, z);
        this.ColorIndex = cIn;
    }
}