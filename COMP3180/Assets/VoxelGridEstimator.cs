using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VoxelGridEstimator : MonoBehaviour
{
    private MeshFilter filter;
    private Mesh mesh;

    private float gridSize = 0f;

    int from, to;

    // Start is called before the first frame update
    void Start()
    {
        filter = GetComponent<MeshFilter>();
        mesh = filter.mesh;
    }


    private void OnGUI()
    {
        bool pressed = GUI.Button(new Rect(16, 16, 100, 35), "Refresh");
        if (pressed)
        {
            UpdateGrid();
        }
    }

    SlicedMesh SliceMesh()
    {
        return new SlicedMesh();
    }

    void UpdateGrid()
    {
        if (filter == null)
        {
            filter = GetComponent<MeshFilter>();
        }

        Debug.Log("Getting size");

        // Find smallest distance between 2 vertices
        float smallestDistance = float.MaxValue;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            from = i;
            for (int j = 0; j < mesh.vertexCount; j++)
            {
                to = j;
                if (i != j)
                {
                    float dist = Vector3.Distance(mesh.vertices[i], mesh.vertices[j]);
                    if (dist < smallestDistance)
                    {
                        smallestDistance = dist;
                    }
                }
            }
        }
        if (smallestDistance < float.MaxValue)
        {
            gridSize = smallestDistance;

        }

        Debug.Log("Got distance " + gridSize);
    }

    private void Update()
    {
        Debug.DrawLine(mesh.vertices[from], mesh.vertices[to], Color.red);
    }

    private void OnDrawGizmos()
    {
        if (filter == null)
        {
            filter = GetComponent<MeshFilter>();
        }

        for (int i = 0; i < filter.sharedMesh.vertexCount; i++)
        {
            Gizmos.DrawSphere(filter.sharedMesh.vertices[i], 0.1f);
        }
    }
}
