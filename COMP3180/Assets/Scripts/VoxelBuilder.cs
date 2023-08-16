using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class VoxelBuilder : MonoBehaviour
{
    [SerializeField] private TextAsset voxFile;

    [SerializeField] private float voxelSize = 0.25f;

    private Dictionary<Vector3Int, Voxel> mappedVoxels = new Dictionary<Vector3Int, Voxel>();
    private MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        LoadVoxel();
        BuildVoxel();
    }

    public void RemoveVox(Voxel vox)
    {
        if (mappedVoxels.ContainsKey(vox.LocalPosition))
        {
            mappedVoxels.Remove(vox.LocalPosition);
            BuildVoxel();
        }
    }

    public void BuildVoxel()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> norms = new List<Vector3>();

        foreach (KeyValuePair<Vector3Int, Voxel> vox in mappedVoxels)
        {
            FaceDirections fd = new FaceDirections(
                 !mappedVoxels.ContainsKey(vox.Value.LocalPosition + new Vector3Int(1, 0, 0)),
                 !mappedVoxels.ContainsKey(vox.Value.LocalPosition + new Vector3Int(-1, 0, 0)),
                 !mappedVoxels.ContainsKey(vox.Value.LocalPosition + new Vector3Int(0, 1, 0)),
                 !mappedVoxels.ContainsKey(vox.Value.LocalPosition + new Vector3Int(0, -1, 0)),
                 !mappedVoxels.ContainsKey(vox.Value.LocalPosition + new Vector3Int(0, 0, 1)),
                 !mappedVoxels.ContainsKey(vox.Value.LocalPosition + new Vector3Int(0, 0, -1)));

            vox.Value.Build(out Vector3[] v, out int[] t, out Vector3[] n, fd);

            for (int i = 0; i < t.Length; i++)
            {
                tris.Add(t[i] + verts.Count);
            }
            for (int i = 0; i < v.Length; i++)
            {
                verts.Add(vox.Value.Position + v[i]);
            }
            norms.AddRange(n);
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetNormals(norms);

        meshFilter.mesh = mesh;
    }

    public void LoadVoxel()
    {
        // read vox file data
        string[] splitData = voxFile.text.Split(';');

        mappedVoxels = new Dictionary<Vector3Int, Voxel>();

        for (int i = 0; i < splitData.Length; i++)
        {
            if (splitData[i].Length > 0)
            {
                string[] pos = splitData[i].Split(',');

                Vector3Int localPos = new Vector3Int(int.Parse(pos[0]), int.Parse(pos[1]), int.Parse(pos[2]));
                Vector3Rounded voxPos = new Vector3Rounded(localPos, voxelSize);
                Voxel newVoxel = new Voxel(voxPos, localPos);
                if (!mappedVoxels.ContainsKey(localPos))
                {
                    mappedVoxels.Add(localPos, newVoxel);
                }
            }
        }

        // CPU
        

        // based on unique voxels, create verts non overlapping

        // build tris

        // create mesh 

        // assign mesh

        // GPU
        // split into 8x8 grids

        // write data to bool array

        // pass to gpu

        // get verts + tris back from gpu

        // create mesh

        // asign mesh
    }
}
