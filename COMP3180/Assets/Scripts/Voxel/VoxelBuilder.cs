using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class VoxelBuilder : MonoBehaviour
{
    [SerializeField] private TextAsset voxFile;

    [SerializeField] private float voxelSize = 0.25f;

    private Dictionary<Vector3Int, Voxel> mappedVoxels = new Dictionary<Vector3Int, Voxel>();
    private List<Color> mappedColors = new List<Color>();

    private MeshFilter meshFilter;


    [ContextMenu("Edit Model")]
    public void EditModel()
    {
        VoxelEditor.ShowVoxelEditor(this);
    }

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

    public void UpdateVoxel(Voxel vox)
    {
        // update surrounding voxels
    }

    private void OnValidate()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        LoadVoxel();
        BuildVoxel();
    }

    public void BuildVoxel()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> norms = new List<Vector3>();
        List<Color> cols = new List<Color>();

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

                Color c = new Color(1, 0, 1, 1);
                if (vox.Value.ColorIndex >= 0 && vox.Value.ColorIndex < mappedColors.Count)
                {
                    c = mappedColors[vox.Value.ColorIndex];
                }
                cols.Add(c);
            }
            norms.AddRange(n);
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetNormals(norms);
        mesh.SetColors(cols);

        meshFilter.sharedMesh = mesh;
    }

    public void LoadVoxel()
    {
        mappedVoxels.Clear();
        mappedColors.Clear();
        
        string contents = voxFile.text;

        bool writeVertex = true;
        int[] data = new int[4];
        int dataIndex = 0;
        string rawData = "";

        for (int i = 0; i < contents.Length; i++)
        {
            switch (contents[i])
            {
                case 'v':
                    writeVertex = true;
                    break;
                
                case 'c':
                    writeVertex = false;
                    break;
                
                case ',':
                    int.TryParse(rawData, out data[dataIndex]);
                    dataIndex++;
                    rawData = "";
                    break;

                case ';':
                    int.TryParse(rawData, out data[dataIndex]);

                    if (writeVertex)
                    {
                        Vector3Int pos = new Vector3Int(data[0], data[1], data[2]);
                        if (!mappedVoxels.ContainsKey(pos))
                        {
                            mappedVoxels.Add(pos, new Voxel(new Vector3Rounded(pos, voxelSize), pos, data[3]));
                        }
                    }
                    else
                    {
                        mappedColors.Add(new Color(data[0] / 255f, data[1] / 255f, data[2] / 255f, data[3] / 255));
                    }

                    dataIndex = 0;
                    rawData = "";
                    break;
                
                    default:
                    rawData += contents[i];
                    break;
            }
        }

        // read vox file data
/*        string[] splitData = voxFile.text.Split(';');

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
        }*/
    }
}
