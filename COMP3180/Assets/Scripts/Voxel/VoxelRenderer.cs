using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class VoxelRenderer : MonoBehaviour
{
    private VoxelBuilder voxelBuilder = new VoxelBuilder();

    public VoxelData VoxelData
    {
        get => voxelData;
        set
        {
            if (value == voxelData)
            {
                return;
            }

            voxelData = value;
            UpdateMesh();
        }
    }

    public VoxelShape VoxelShape
    {
        get => voxelShape;
        set
        {
            if (value == voxelShape)
            {
                return;
            }

            voxelShape = value;
            UpdateMesh();
        }
    }

    private VoxelData voxelData;
    private VoxelShape voxelShape;
    private MeshFilter meshFilter;

    public bool UpdateMesh()
    {
        if (meshFilter == null)
        {
            if (!TryGetComponent(out MeshFilter mf))
            {
                return false;
            }

            meshFilter = mf;
        }

        if (voxelData == null || voxelShape == null)
        {
            return false;
        }

        Mesh mesh = voxelBuilder.Build(voxelData, voxelShape);
        meshFilter.sharedMesh = mesh;
        return true;
    }
}
