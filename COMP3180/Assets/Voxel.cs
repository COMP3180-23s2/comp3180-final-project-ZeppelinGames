using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Voxel
{
	public Vector3Rounded Position => position;

	private Vector3Rounded position;
	private float halfSize;

	public Voxel(Vector3Rounded position)
	{
		this.halfSize = position.round  / 2.0f;
		this.position = position;
	}

	public Voxel(int x, int y, int z, int size) : this(new Vector3Rounded(x,y,z, size)) {}

	public void Build(out Vector3[] verts, out int[] tris, out Vector3[] norms, bool XP = true, bool XN = true, bool YP = true, bool YN = true, bool ZP = true, bool ZN = true)
	{
		List<int> triangles = new List<int>();
		// cube tris
		/*	int[] triangles = {
				0, 2, 1, //face front
				0, 3, 2,

				2, 3, 4, //face top
				2, 4, 5,

				1, 2, 5, //face right
				1, 5, 6,

				0, 7, 4, //face left
				0, 4, 3,

				5, 4, 7, //face back
				5, 7, 6,

				0, 6, 7, //face bottom
				0, 1, 6
			};*/

		if (XP)
		{
			triangles.AddRange(new int[] { 1, 2, 5, 1, 5, 6 });
		}

		if (XN)
		{
			triangles.AddRange(new int[] { 0, 7, 4, 0, 4, 3 });
		}

		if (YP)
		{
			triangles.AddRange(new int[] { 2, 3, 4, 2, 4, 5 });
		}

		if (YN)
		{
			triangles.AddRange(new int[] { 0, 6, 7, 0, 1, 6 });
		}

		if (ZP)
		{
			triangles.AddRange(new int[] { 5, 4, 7, 5, 7, 6 });
		}

		if (ZN)
		{
			triangles.AddRange(new int[] { 0, 2, 1, 0, 3, 2 });
		}

		// centered cube verts
		verts = new Vector3[] {
			new Vector3 (-halfSize, -halfSize, -halfSize),
			new Vector3 (halfSize, -halfSize, -halfSize),
			new Vector3 (halfSize, halfSize, -halfSize),
			new Vector3 (-halfSize, halfSize, -halfSize),
			new Vector3 (-halfSize, halfSize, halfSize),
			new Vector3 (halfSize, halfSize, halfSize),
			new Vector3 (halfSize, -halfSize, halfSize),
			new Vector3 (-halfSize, -halfSize, halfSize),
		};
		norms = new Vector3[]
		{
			new Vector3 (-halfSize, -halfSize, -halfSize),
			new Vector3 (halfSize, -halfSize, -halfSize),
			new Vector3 (halfSize, halfSize, -halfSize),
			new Vector3 (-halfSize, halfSize, -halfSize),
			new Vector3 (-halfSize, halfSize, halfSize),
			new Vector3 (halfSize, halfSize, halfSize),
			new Vector3 (halfSize, -halfSize, halfSize),
			new Vector3 (-halfSize, -halfSize, halfSize),
		};

		tris = triangles.ToArray();
	}
}