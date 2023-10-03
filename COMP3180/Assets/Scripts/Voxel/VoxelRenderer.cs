using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class VoxelRenderer : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TextAsset voxelDataFile;
    [SerializeField] private bool overrideShape = false;
    [SerializeField] private TextAsset voxelShapeFile;

    [Header("Material")]
    [SerializeField] private bool overrideDefaultMaterial = false;
    [SerializeField] private Material mat;

    public VoxelData VoxelData => voxelData;
    public VoxelShape VoxelShape => voxelShape;

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
    public static TextAsset DefaultShape
    {
        get
        {
            if (defaultShape == null)
            {
                defaultShape = Resources.Load<TextAsset>("Voxel/DefaultShape");
            }
            return defaultShape;
        }
    }
    #endregion

    public bool UpdateMesh()
    {
        if (voxelShapeFile == null && voxelDataFile == null)
        {
            Debug.LogWarning("Missing Voxel Data or Shape file(s)");
            return false;
        }

        // Re-read and load data from file
        voxelData = new VoxelData(voxelDataFile);
        voxelShape = new VoxelShape(voxelShapeFile);

        // Update material
        MeshRenderer.material = overrideDefaultMaterial ? mat : DefaultMaterial;

        // Build mesh
        Mesh mesh = VoxelBuilder.Build(voxelData, voxelShape);
        MeshFilter.sharedMesh = mesh;

        return true;
    }
}
