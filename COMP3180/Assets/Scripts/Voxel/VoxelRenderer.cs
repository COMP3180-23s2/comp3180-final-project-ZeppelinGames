using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class VoxelRenderer : MonoBehaviour
{
    public delegate void MeshBuildStartDelegate();
    public MeshBuildStartDelegate meshBuildStarted;

    public delegate void MeshBuildCompleteDelegate();
    public MeshBuildCompleteDelegate meshBuildComplete;

    [Header("Data")]
    [SerializeField] private TextAsset voxelDataFile;
    public TextAsset VoxelDataFile
    {
        get
        {
            return voxelDataFile;
        }
        set
        {
            voxelDataFile = value;
        }
    }

    [SerializeField] private bool overrideShape = false;
    [SerializeField] private TextAsset voxelShapeFile;

    [Header("Material")]
    [SerializeField] private bool overrideDefaultMaterial = false;
    [SerializeField] private Material mat;
    public Material Material
    {
        get
        {
            return mat == null || (mat != null && !overrideDefaultMaterial) ? DefaultMaterial : mat;
        }
    }

    public VoxelData VoxelData => voxelData;
    public VoxelShape VoxelShape
    {
        get
        {
            return voxelShape == null ? DefaultShape : voxelShape;
        }
    }

    private VoxelData voxelData;
    private VoxelShape voxelShape;

    // Mesh Components
    #region MESH
    private Mesh voxelMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    public MeshFilter MeshFilter
    {
        get
        {
            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }
            return meshFilter;
        }
    }

    public MeshRenderer MeshRenderer
    {
        get
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }
            return meshRenderer;
        }
    }
    #endregion

    // Static Varibles / Defaults
    #region STATICS
    private static Material defaultMaterial;
    public static Material DefaultMaterial
    {
        get
        {
            if (defaultMaterial == null)
            {
                defaultMaterial = Resources.Load<Material>("Voxel/VertexColor");
            }
            return defaultMaterial;
        }
    }
    private static TextAsset defaultShape;
    public static VoxelShape DefaultShape
    {
        get
        {
            if (defaultShape == null)
            {
                defaultShape = Resources.Load<TextAsset>("Voxel/DefaultShape");
            }
            return new VoxelShape(defaultShape);
        }
    }
    #endregion


    private void OnEnable()
    {
        voxelMesh = new Mesh();
        voxelMesh.name = "Voxel";
        MeshFilter.sharedMesh = voxelMesh;
    }

    private void Start()
    {
        if (voxelDataFile != null)
        {
            LoadMesh();
            BuildMesh();
        }
    }

    public bool LoadMesh()
    {
        if (voxelDataFile == null)
        {
            Debug.LogWarning("Missing Voxel Data file");
            return false;
        }

        // Re-read and load data from file
        voxelData = new VoxelData(voxelDataFile);
        if (voxelShapeFile != null)
        {
            voxelShape = new VoxelShape(voxelShapeFile);
        }
        return true;
    }

    public void UpdateVoxelData(VoxelData vd = null, VoxelShape vs = null)
    {
        if (vd != null)
        {
            voxelData = vd;
        }
        if (vs != null)
        {
            voxelShape = vs;
        }
    }

    public bool BuildMesh(VoxelData vd = null, VoxelShape vs = null)
    {
        meshBuildStarted?.Invoke();

        UpdateVoxelData(vd, vs);

        if (VoxelData == null || VoxelShape == null)
        {
            Debug.LogWarning("Mesh build failed. Invalid voxel data or shape");
            return false;
        }

        if (overrideDefaultMaterial || MeshRenderer.sharedMaterial == null)
        {
            // Update material.
            MeshRenderer.sharedMaterial = Material;
        }

        VoxelBuilder.Build(VoxelData, VoxelShape, out Vector3[] v, out int[] t, out Vector3[] n, out Color[] c);

        if (voxelMesh == null)
        {
            voxelMesh = new Mesh();
            voxelMesh.name = "VoxelMesh";
            MeshFilter.mesh = voxelMesh;
        }

        voxelMesh.SetVertices(v);
        voxelMesh.SetTriangles(t, 0);
        voxelMesh.SetNormals(n);
        voxelMesh.SetColors(c);


        // Update other linked components
        meshBuildComplete?.Invoke();
        return true;
    }

    public bool EditorBuildMesh(VoxelData vd = null, VoxelShape vs = null)
    {
        meshBuildStarted?.Invoke();

        UpdateVoxelData(vd, vs);

        if (VoxelData == null || VoxelShape == null)
        {
            Debug.LogWarning("Mesh build failed. Invalid voxel data or shape");
            return false;
        }

        if (overrideDefaultMaterial || MeshRenderer.sharedMaterial == null)
        {
            // Update material.
            MeshRenderer.sharedMaterial = Material;
        }

        VoxelBuilder.Build(VoxelData, VoxelShape, out Vector3[] v, out int[] t, out Vector3[] n, out Color[] c);

        voxelMesh.Clear();

        voxelMesh.SetVertices(v);
        voxelMesh.SetTriangles(t, 0);
        voxelMesh.SetNormals(n);
        voxelMesh.SetColors(c);

        // Update other linked components
        meshBuildComplete?.Invoke();
        return true;
    }

    public void GroupAndFracture()
    {
        if (VoxelData.VoxelPoints.Length <= 0)
        {
            return;
        }

        // pick point

        // recusively check for neighbours
        //StartCoroutine(PlzDontBlowUp());

        Vector3Int[] points = new Vector3Int[VoxelData.VoxelPoints.Length];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = VoxelData.VoxelPoints[i].Position;
        }
        List<List<VoxelPoint>> grouped = GroupConnected(VoxelData.VoxelPoints);
        Debug.Log($"Groups: {grouped.Count}");

        if (TryGetComponent(out VoxelCollider vc))
        {
            vc.enabled = false;
        }
        for (int i = 0; i < grouped.Count; i++)
        {
            VoxelBuilder.NewRenderer(grouped[i].ToArray(), VoxelData.Colors, this.transform);
        }
        this.gameObject.SetActive(false);
    }

    private Vector3Int[] NeighbourOffsets = new Vector3Int[]
    {
            new Vector3Int(1,0,0),
            new Vector3Int(-1,0,0),
            new Vector3Int(0,1,0),
            new Vector3Int(0,-1,0),
            new Vector3Int(0,0,1),
            new Vector3Int(0,0,-1)
    };

    public List<List<VoxelPoint>> GroupConnected(VoxelPoint[] vector3Ints)
    {
        List<List<VoxelPoint>> groups = new List<List<VoxelPoint>>();
        HashSet<VoxelPoint> visited = new HashSet<VoxelPoint>();

        foreach (VoxelPoint vector in vector3Ints)
        {
            if (!visited.Contains(vector))
            {
                List<VoxelPoint> group = new List<VoxelPoint>();
                DepthFirstSearch(vector, vector3Ints, visited, group);
                groups.Add(group);
            }
        }

        return groups;
    }

    private void DepthFirstSearch(VoxelPoint vector, VoxelPoint[] vector3Ints, HashSet<VoxelPoint> visited, List<VoxelPoint> group)
    {
        visited.Add(vector);
        group.Add(vector);

        foreach (VoxelPoint neighbor in GetNeighbors(vector, vector3Ints))
        {
            if (!visited.Contains(neighbor))
            {
                DepthFirstSearch(neighbor, vector3Ints, visited, group);
            }
        }
    }

    private List<VoxelPoint> GetNeighbors(VoxelPoint vector, VoxelPoint[] vector3Ints)
    {
        List<VoxelPoint> neighbors = new List<VoxelPoint>();

        foreach (Vector3Int offset in NeighbourOffsets)
        {
            Vector3Int neighbor = vector.Position + offset;
            if (Array.Exists(vector3Ints, v => v.Position == neighbor))
            {
                //Get vp from pos
                //neighbors.Add(neighbor);
                VoxelPoint? vp = VoxelPointFromPosition(neighbor);
                if (vp != null)
                {
                    neighbors.Add(vp.Value);
                }
            }
        }

        return neighbors;
    }

    private VoxelPoint? VoxelPointFromPosition(Vector3Int position)
    {
        for (int i = 0; i < VoxelData.VoxelPoints.Length; i++)
        {
            if (VoxelData.VoxelPoints[i].Position == position)
            {
                return VoxelData.VoxelPoints[i];
            }
        }
        return null;
    }
}
