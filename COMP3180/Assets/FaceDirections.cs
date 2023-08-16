using System;
using UnityEngine;
public struct FaceDirections
{
	public bool[] faces;
	public FaceDirections(bool XP = true, bool XN = true, bool YP = true, bool YN = true, bool ZP = true, bool ZN = true)
	{
		faces = new bool[]{
			YP,YN,XP,XN,ZP,ZN
		};
	}
}
