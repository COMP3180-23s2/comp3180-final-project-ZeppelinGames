using System;
using UnityEngine;

public struct FaceDirections
{
	public bool[] faces;
	public int FaceCount => faceCount;

	private int faceCount;

	public FaceDirections(bool XP = true, bool XN = true, bool YP = true, bool YN = true, bool ZP = true, bool ZN = true)
	{
		faces = new bool[]{
			YP,YN,XP,XN,ZP,ZN
		};

		this.faceCount = 0;
		for (int i = 0; i < faces.Length; i++)
		{
			if (faces[i])
			{
				this.faceCount++;
			}
		}
	}
}
