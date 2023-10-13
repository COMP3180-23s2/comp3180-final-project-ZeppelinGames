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
    #region Mesh
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

        if (MeshFilter.sharedMesh == null) { MeshFilter.sharedMesh = new Mesh(); }

        MeshFilter.mesh.Clear();
            
        MeshFilter.mesh.SetVertices(v);
        MeshFilter.mesh.SetTriangles(t, 0);
        MeshFilter.mesh.SetNormals(n);
        MeshFilter.mesh.SetColors(c);

        // Update other linked components
        meshBuildComplete?.Invoke();
        return true;
    }
}
