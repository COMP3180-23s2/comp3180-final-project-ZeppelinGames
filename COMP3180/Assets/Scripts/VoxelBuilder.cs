using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class VoxelBuilder : MonoBehaviour
{
    [SerializeField] private TextAsset voxFile;

    [SerializeField] private float voxelSize = 0.25f;

    private Dictionary<Vector3Rounded, Voxel> mappedVoxels = new Dictionary<Vector3Rounded, Voxel>();
    private MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        LoadVoxel();
        BuildVoxel();
    }

    public void RemoveVox(Vector3Rounded voxelPosition)
    {
        if (mappedVoxels.ContainsKey(voxelPosition))
        {
            mappedVoxels.Remove(voxelPosition);
            BuildVoxel();
        }
    }

    public void BuildVoxel()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> norms = new List<Vector3>();

        foreach (KeyValuePair<Vector3Rounded, Voxel> vox in mappedVoxels)
        {
            vox.Value.Build(out Vector3[] v, out int[] t, out Vector3[] n,
                XP: !mappedVoxels.ContainsKey(vox.Value.Position + new Vector3Rounded(1, 0, 0, voxelSize)),
                XN: !mappedVoxels.ContainsKey(vox.Value.Position + new Vector3Rounded(-1, 0, 0, voxelSize)),
                YP: !mappedVoxels.ContainsKey(vox.Value.Position + new Vector3Rounded(0, 1, 0, voxelSize)),
                YN: !mappedVoxels.ContainsKey(vox.Value.Position + new Vector3Rounded(0, -1, 0, voxelSize)),
                ZP: !mappedVoxels.ContainsKey(vox.Value.Position + new Vector3Rounded(0, 0, 1, voxelSize)),
                ZN: !mappedVoxels.ContainsKey(vox.Value.Position + new Vector3Rounded(0, 0, -1, voxelSize)));

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

        mappedVoxels = new Dictionary<Vector3Rounded, Voxel>();

        for (int i = 0; i < splitData.Length; i++)
        {
            if (splitData[i].Length > 0)
            {
                string[] pos = splitData[i].Split(',');

                Vector3Rounded voxPos = new Vector3Rounded(int.Parse(pos[0]), int.Parse(pos[1]), int.Parse(pos[2]), voxelSize);
                Voxel newVoxel = new Voxel(voxPos);
                if (!mappedVoxels.ContainsKey(voxPos))
                {
                    mappedVoxels.Add(voxPos, newVoxel);
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
