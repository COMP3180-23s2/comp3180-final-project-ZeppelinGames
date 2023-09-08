using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VoxelShape
{
    public Vector3[] Vertices => verts;
    public Vector3[] Normals => norms;
    public int[][] FaceTriangles => faceTriangles;

    private TextAsset voxelShapeFile;
    private Vector3[] verts;
    private Vector3[] norms;

    private int[][] faceTriangles = new int[6][];

    public VoxelShape(TextAsset voxelShapeFile)
    {
        this.voxelShapeFile = voxelShapeFile;
        VoxParser.Parse(
            voxelShapeFile.text,
            out verts,
            out _, out _,
            out norms,
            out _,
            out faceTriangles[0],
            out faceTriangles[1],
            out faceTriangles[2],
            out faceTriangles[3],
            out faceTriangles[4],
            out faceTriangles[5]);
    }

    public VoxelShape(Vector3[] verts, Vector3[] norms, int[][] tris)
    {
        this.verts = verts;
        this.norms = norms;
        this.faceTriangles = tris;
    }
}
