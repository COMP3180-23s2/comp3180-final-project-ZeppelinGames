using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelBuilder
{
    private float voxelSize = 1f;

    // Voxel Data 
    private List<Vector3Int> positionKeys = new List<Vector3Int>();
    private Dictionary<Vector3Int, Voxel> mappedVoxels = new Dictionary<Vector3Int, Voxel>();
    private List<Color> mappedColors = new List<Color>();

    // Voxel Shape

    private TextAsset voxelDataFile;
    private TextAsset voxelShapeFile;

    public VoxelBuilder(TextAsset voxelData, TextAsset voxelShape, float voxelSize = 1f)
    {
        this.voxelSize = 1f;
        this.voxelDataFile = voxelData;
        this.voxelShapeFile = voxelShape;

        VoxParser.Parse(voxelDataFile.text, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _);
        VoxParser.Parse(voxelShapeFile.text, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _);
    }
}
