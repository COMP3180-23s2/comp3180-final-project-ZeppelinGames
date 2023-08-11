using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
public class VoxelGridEstimator : MonoBehaviour
{
    private MeshFilter filter;
    private Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        filter = GetComponent<MeshFilter>();
        mesh = filter.mesh;
    }


    private void OnGUI()
    {
        bool pressed = GUI.Button(new Rect(16, 16, 100, 35), "Cut mesh");
        if (pressed)
        {
            CutMesh();
        }
    }

    void CutMesh()
    {
        Plane p = new Plane(Random.insideUnitSphere, 1);

        // Get split mesh vertices
        List<Vector3> vertsA = new List<Vector3>();
        List<Vector3> vertsB = new List<Vector3>();

        Dictionary<int, int> vertsAIndexMap = new Dictionary<int, int>();
        Dictionary<int, int> vertsBIndexMap = new Dictionary<int, int>();

        bool[] side = new bool[mesh.vertexCount];
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            side[i] = p.GetSide(mesh.vertices[i]);

            if (side[i])
            {
                vertsAIndexMap.Add(i, vertsA.Count);
                vertsA.Add(mesh.vertices[i]);
            }
            else
            {
                vertsBIndexMap.Add(i, vertsB.Count);
                vertsB.Add(mesh.vertices[i]);
            }
        }

        // Get triangles connected to new verts
        List<int> trisA = new List<int>();
        List<int> trisB = new List<int>();

        // Generate tris
        for (int i = 0; i < vertsA.Count; i++)
        {
            GetClosestVerts(vertsA.ToArray(), vertsA[i], out Vector3[] closest);

            trisA.Add(i);
            trisA.Add(vertsA.IndexOf(closest[1]));
            trisA.Add(vertsA.IndexOf(closest[2]));

            trisA.Add(i);
            trisA.Add(vertsA.IndexOf(closest[2]));
            trisA.Add(vertsA.IndexOf(closest[1]));
        }

        // Create new meshes
        CreateCutMesh(vertsA.ToArray(), trisA.ToArray());
        CreateCutMesh(vertsB.ToArray(), trisB.ToArray());

        gameObject.SetActive(false);
    }

    private void GetClosestVerts(Vector3[] verts, Vector3 to, out Vector3[] closest)
    {
        closest = verts.OrderBy(i => Vector3.Distance(i, to)).ToArray();
    }

    private GameObject CreateCutMesh(Vector3[] verts, int[] tris)
    {
        GameObject newGO = new GameObject("Cut");
        MeshRenderer mr = newGO.AddComponent<MeshRenderer>();
        MeshFilter mf = newGO.AddComponent<MeshFilter>();

        Mesh newMesh = new Mesh();
        newMesh.vertices = verts;
        newMesh.triangles = tris;

        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();

        mf.mesh = newMesh;

        return newGO;
    }

    private void OnDrawGizmos()
    {
        if (filter == null)
        {
            filter = GetComponent<MeshFilter>();
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < filter.sharedMesh.vertexCount; i++)
        {
            Gizmos.DrawSphere(transform.position + filter.sharedMesh.vertices[i], 0.1f);
        }
    }
}
