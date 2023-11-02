using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private TextAsset voxelShapeFile;
    [SerializeField] private VoxelBreakType breakType;
    public VoxelBreakType BreakType => breakType;

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

    private void OnValidate()
    {
        if (voxelDataFile != null)
        {
            InitMesh();
            LoadMesh();
            BuildMesh();
        }
    }

    private void Start()
    {
        if (voxelDataFile != null)
        {
            InitMesh();
            LoadMesh();
            BuildMesh();
        }
    }

    public void InitMesh()
    {
        if (voxelMesh == null)
        {
            voxelMesh = new Mesh();
            voxelMesh.name = "Voxel";
            MeshFilter.sharedMesh = voxelMesh;
        }
    }

    public bool LoadMesh()
    {
        if (voxelDataFile == null)
        {
            Debug.LogWarning("Missing voxel data file");
            return false;
        }

        // Re-read and load data from file
        voxelData = new VoxelData(VoxelDataFile);
        voxelShape = voxelShapeFile != null ? new VoxelShape(voxelShapeFile) : VoxelShape;

        return true;
    }

    public void UpdateMaterial(Material m)
    {
        this.mat = m;
        overrideDefaultMaterial = true;
        MeshRenderer.material = Material;
    }

    public void UpdateBreakType(VoxelBreakType breakType)
    {
        this.breakType = breakType;
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
            if (!LoadMesh())
            {
                return false;
            }
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

        voxelMesh.Clear();
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

        Vector3Int[] points = new Vector3Int[VoxelData.VoxelPoints.Length];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = VoxelData.VoxelPoints[i].Position;
        }
        List<List<VoxelPoint>> grouped = GroupConnected(VoxelData.VoxelPoints);

        if (grouped.Count > 0)
        {
            this.BuildMesh(new VoxelData(grouped[0].ToArray(), VoxelData.Colors));
            for (int i = 1; i < grouped.Count; i++)
            {
                VoxelBuilder.NewRenderer(grouped[i].ToArray(), VoxelData.Colors, out Rigidbody _, this.transform, this);
            }
        }
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

        VoxelPoint[] neighbours = GetNeighbours(vector.Position, out int nCount);
        for (int i = 0; i < nCount; i++)
        {
            if (!visited.Contains(neighbours[i]))
            {
                DepthFirstSearch(neighbours[i], vector3Ints, visited, group);
            }
        }
    }

    public List<VoxelPoint> GetNeighbours(VoxelPoint vector)
    {
        List<VoxelPoint> neighbors = new List<VoxelPoint>();

        foreach (Vector3Int offset in NeighbourOffsets)
        {
            Vector3Int neighbor = vector.Position + offset;
            if (voxelData.VoxelMap.ContainsKey(neighbor))
            {
                neighbors.Add(voxelData.VoxelMap[neighbor]);
            }
        }

        return neighbors;
    }

    public VoxelPoint[] GetNeighbours(Vector3Int v, out int neighbourCount)
    {
        VoxelPoint[] neighbours = new VoxelPoint[9];
        neighbourCount = 0;

        foreach (Vector3Int offset in NeighbourOffsets)
        {
            Vector3Int neighbour = v + offset;
            if (voxelData.VoxelMap.ContainsKey(neighbour))
            {
                neighbours[neighbourCount] = voxelData.VoxelMap[neighbour];
                neighbourCount++;
            }
        }
        return neighbours;
    }

    public VoxelPoint? VoxelPointFromPosition(Vector3Int position)
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

    public VoxelPoint ClosestPointTo(Vector3 position)
    {
        //defaults to first voxel if none?
        int closest = 0;
        float closestDist = float.MaxValue;
        for (int i = 0; i < VoxelData.VoxelPoints.Length; i++)
        {
            float dist = Vector3.Distance(VoxelData.VoxelPoints[i].WorldPosition(transform), position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = i;
            }
        }
        return VoxelData.VoxelPoints[closest];
    }

    public Vector3Int WorldToLocalVoxel(Vector3 world)
    {
        Vector3 rounded = RoundToVoxelPosition(world - transform.position);

        Vector3 inv = transform.InverseTransformVector(rounded) / VoxelBuilder.VoxelSize;
        return new Vector3Int(
            Mathf.RoundToInt(inv.x),
            Mathf.RoundToInt(inv.y),
            Mathf.RoundToInt(inv.z)
        );
    }

    public Vector3 RoundToVoxelPosition(Vector3 v)
    {
        return new Vector3(
                Mathf.Round(v.x / VoxelBuilder.VoxelSize) * VoxelBuilder.VoxelSize,
                Mathf.Round(v.y / VoxelBuilder.VoxelSize) * VoxelBuilder.VoxelSize,
                Mathf.Round(v.z / VoxelBuilder.VoxelSize) * VoxelBuilder.VoxelSize
        );
    }

    public Vector3 LocalToWorldVoxel(Vector3Int local)
    {
        return transform.TransformVector((Vector3)local * VoxelBuilder.VoxelSize);
    }

    public Vector3 Inv(Vector3Int v)
    {
        return transform.InverseTransformPoint(v) / VoxelBuilder.VoxelSize;
    }
}
