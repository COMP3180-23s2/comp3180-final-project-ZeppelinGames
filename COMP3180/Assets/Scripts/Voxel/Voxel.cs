using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Voxel
{
    public Vector3Rounded Position => position;
    public Vector3Int LocalPosition => localPosition;
    public int ColorIndex => colorIndex;
    public FaceDirections FaceDirs => faceDirs;

    private Vector3Rounded position;
    private Vector3Int localPosition;
    private int colorIndex;
    private float halfSize;
    private FaceDirections faceDirs;

    public Voxel(Vector3Rounded position, Vector3Int localPosition)
    {
        this.localPosition = localPosition;
        this.halfSize = position.round / 2.0f;
        this.position = position;
    }

    public Voxel(int x, int y, int z, int size) : this(new Vector3Rounded(x, y, z, size), new Vector3Int(x, y, z)) { }
    
    public Voxel(Vector3Rounded position, Vector3Int localPosition, int c) : this(position, localPosition)
    {
        this.colorIndex = c;
    }

    readonly static int[][] faceTriangles = new int[][]
    {
		// Top face
		new int[] { 0, 2, 3, 3, 1, 0 },

		// Bottom face
		new int[] { 7, 6, 4, 4, 5, 7 },
		
		// X-Pos
		new int[] { 8, 11, 9, 8, 10, 11 },
		
		// X-Neg
		new int[] { 12, 13, 15, 12, 15, 14 },
		
		// Z-Pos
		new int[] { 19, 18, 16, 17, 19, 16 },
		
		// Z-Neg
		new int[] { 20, 22, 23, 20, 23, 21 }
    };

    public readonly static Vector3[] norms = new Vector3[]
    {
        Vector3.up,
        Vector3.up,
        Vector3.up,
        Vector3.up,

        Vector3.down,
        Vector3.down,
        Vector3.down,
        Vector3.down,

        Vector3.right,
        Vector3.right,
        Vector3.right,
        Vector3.right,

        Vector3.left,
        Vector3.left,
        Vector3.left,
        Vector3.left,

        Vector3.forward,
        Vector3.forward,
        Vector3.forward,
        Vector3.forward,

        Vector3.back,
        Vector3.back,
        Vector3.back,
        Vector3.back,
    };

    public void Build(out Vector3[] verts, out int[] tris, out Vector3[] norms, FaceDirections directions)
    {
        this.faceDirs = directions;
        List<int> triangles = new List<int>();

        for (int i = 0; i < directions.faces.Length; i++)
        {
            if (directions.faces[i])
            {
                triangles.AddRange(faceTriangles[i]);
            }
        }
        tris = triangles.ToArray();

        // centered cube verts
        verts = new Vector3[] {
			// Top Face
			new Vector3(halfSize, halfSize, halfSize),
            new Vector3(-halfSize, halfSize, halfSize),
            new Vector3(halfSize, halfSize, -halfSize),
            new Vector3(-halfSize, halfSize, -halfSize),

			// Bottom Face
			new Vector3(halfSize, -halfSize, halfSize),
            new Vector3(-halfSize, -halfSize, halfSize),
            new Vector3(halfSize, -halfSize, -halfSize),
            new Vector3(-halfSize, -halfSize, -halfSize),

			// X-Pos
			new Vector3(halfSize, halfSize, halfSize),
            new Vector3(halfSize, halfSize, -halfSize),
            new Vector3(halfSize, -halfSize, halfSize),
            new Vector3(halfSize, -halfSize, -halfSize),

			// X-Neg
			new Vector3(-halfSize, halfSize, halfSize),
            new Vector3(-halfSize, halfSize, -halfSize),
            new Vector3(-halfSize, -halfSize, halfSize),
            new Vector3(-halfSize, -halfSize, -halfSize),

			// Z-Pos
			new Vector3(halfSize, halfSize, halfSize),
            new Vector3(-halfSize, halfSize, halfSize),
            new Vector3(halfSize, -halfSize, halfSize),
            new Vector3(-halfSize, -halfSize, halfSize),

			// Z-Neg
			new Vector3(halfSize, halfSize, -halfSize),
            new Vector3(-halfSize, halfSize, -halfSize),
            new Vector3(halfSize, -halfSize, -halfSize),
            new Vector3(-halfSize, -halfSize, -halfSize),
        };

        norms = Voxel.norms;
    }
}