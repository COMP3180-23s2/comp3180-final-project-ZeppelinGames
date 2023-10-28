using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VoxelData
{
    public Dictionary<Vector3Int, VoxelPoint> VoxelMap => voxelMap;
    private Dictionary<Vector3Int, VoxelPoint> voxelMap = new Dictionary<Vector3Int, VoxelPoint>();

    public Vector3Int[] VoxelPositions => voxelPositions;
    private Vector3Int[] voxelPositions;

    public VoxelPoint[] VoxelPoints => voxelPoints;
    private VoxelPoint[] voxelPoints;
    
    public Color[] Colors => colors;
    private Color[] colors;

    private TextAsset voxelDataFile;

    public VoxelData(TextAsset voxelDataFile)
    {
        this.voxelDataFile = voxelDataFile;
        VoxParser.Parse(
            voxelDataFile.text,
            out _,
            out voxelPoints,
            out _,
            out _,
            out colors,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _);

        UpdateMap();
    }

    public VoxelData(VoxelPoint[] points, Color[] colors)
    {
        this.voxelPoints = points;
        this.colors = colors;

        UpdateMap();
    }

    void UpdateMap()
    {
        voxelMap.Clear();
        voxelPositions = new Vector3Int[voxelPoints.Length];
        for (int i = 0; i < voxelPoints.Length; i++)
        {
            voxelMap.Add(voxelPoints[i].Position, voxelPoints[i]);
            voxelPositions[i] = voxelPoints[i].Position;
        }
    }
}