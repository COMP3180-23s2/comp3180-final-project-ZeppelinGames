using UnityEngine;

public struct VoxelPoint
{
    public Vector3 LocalPosition => (Vector3)this.Position * VoxelBuilder.VoxelSize;
    public Vector3Int Position;
    public int ColorIndex;

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

    public Vector3 WorldPosition(Transform t)
    {
        return t.TransformVector((Vector3)this.Position * VoxelBuilder.VoxelSize);
    }
}