using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicedMesh
{
    private Vector3[] vertices;

    private List<Vector3[]>[] slices;

    public SlicedMesh(Mesh mesh)
    {
        HashSet<Vector3> verticesSet = new HashSet<Vector3>();
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            if (!verticesSet.Contains(mesh.vertices[i]))
            {
                verticesSet.Add(mesh.vertices[i]);
            }
        }
        vertices = new Vector3[verticesSet.Count];
        verticesSet.CopyTo(vertices);


        slices = new List<Vector3[]>[3];
        for (int i = 0; i < slices.Length; i++)
        {
            slices[i] = new List<Vector3[]>();
        }

        for (int i = 0; i < vertices.Length; i++)
        {

        }
    }
}