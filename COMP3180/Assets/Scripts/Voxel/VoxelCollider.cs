using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(VoxelRenderer))]
public class VoxelCollider : MonoBehaviour
{
    private List<BoxCollider> colliders = new List<BoxCollider>();
    private List<int[]> pointsLinks = new List<int[]>();

    private VoxelRenderer voxRenderer;

    public VoxelRenderer Renderer => voxRenderer;

    private void Start()
    {
        BoxCollider[] cols = gameObject.GetComponents<BoxCollider>();
        for (int i = 0; i < cols.Length; i++)
        {
            Destroy(cols[i]);
        }

        voxRenderer = GetComponent<VoxelRenderer>();
        voxRenderer.meshBuildComplete += MeshBuildEnd;
        BuildCollider();
    }

    void MeshBuildEnd()
    {
        BuildCollider();
    }

    public void BuildCollider()
    {
        if (voxRenderer == null)
        {
            voxRenderer = GetComponent<VoxelRenderer>();
        }

        for (int i = 0; i < colliders.Count; i++)
        {
            DestroyImmediate(colliders[i]);
        }
        colliders.Clear();
        pointsLinks.Clear();

        BoxCollider[] cols = gameObject.GetComponents<BoxCollider>();
        for (int i = 0; i < cols.Length; i++)
        {
            DestroyImmediate(cols[i]);
        }

        if (voxRenderer.VoxelData == null)
        {
            return;
        }

        VoxelPoint[] points = voxRenderer.VoxelData.VoxelPoints;
        for (int i = 0; i < points.Length; i++)
        {
            BoxCollider c = GetCollider();
            c.center = points[i].LocalPosition;

            c.size = Vector3.one * VoxelBuilder.VoxelSize;

            pointsLinks.Add(new int[] { i });
        }
    }

#if UNITY_EDITOR
    public void ResetCollidersEditor()
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            DestroyImmediate(colliders[i]);
        }
        colliders.Clear();

        BoxCollider[] cols = gameObject.GetComponents<BoxCollider>();
        for (int i = 0; i < cols.Length; i++)
        {
            DestroyImmediate(cols[i]);
        }
    }

    private void OnDestroy()
    {
        if (EditorApplication.isPlaying)
        {
            return;
        }
        ResetCollidersEditor();
    }
#endif

    BoxCollider GetCollider()
    {
        BoxCollider col = gameObject.AddComponent<BoxCollider>();
        col.hideFlags = HideFlags.HideInInspector;
        colliders.Add(col);
        return col;
    }
}
