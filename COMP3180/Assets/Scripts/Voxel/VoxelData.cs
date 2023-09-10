using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VoxelData
{
    public VoxelPoint[] VoxelPoints => voxelPoints;
    public Color[] Colors => colors;

    private VoxelPoint[] voxelPoints;
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
    }

    public VoxelData(VoxelPoint[] points, Color[] colors)
    {
        this.voxelPoints = points;
        this.colors = colors;
    }
}