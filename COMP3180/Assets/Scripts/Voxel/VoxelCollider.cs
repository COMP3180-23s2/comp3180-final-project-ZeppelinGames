using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(VoxelRenderer))]
public class VoxelCollider : MonoBehaviour
{
    private List<BoxCollider> colliders = new List<BoxCollider>();
    private VoxelRenderer voxRenderer;

    public VoxelRenderer Renderer => voxRenderer;

    private void Start()
    {
        voxRenderer = GetComponent<VoxelRenderer>();
        voxRenderer.meshUpdate += MeshUpdate;
        UpdateCollider();
    }

    void MeshUpdate()
    {
        UpdateCollider();
    }

    public void UpdateCollider()
    {
        if (voxRenderer == null)
        {
            voxRenderer = GetComponent<VoxelRenderer>();
        }

        if (voxRenderer.VoxelData == null)
        {
            Debug.Log($"NO VOXEL DATA ON {this.gameObject.name}");
            return;
        }

        VoxelPoint[] points = voxRenderer.VoxelData.VoxelPoints;
        for (int i = 0; i < colliders.Count; i++)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                Destroy(colliders[i]);
            }
            else
            {
                DestroyImmediate(colliders[i]);
            }
#else
            Destroy(colliders[i]);
#endif
        }
        colliders.Clear();

        for (int i = 0; i < points.Length; i++)
        {
            BoxCollider c = GetCollider();
            c.center = points[i].WorldPosition;
            c.size = Vector3.one * VoxelBuilder.VoxelSize;
        }
    }
    /*
    #if UNITY_EDITOR
        private void OnDestroy()
        {
            if (!EditorApplication.isPlaying)
            {
                for (int i = 0; i < colliders.Count; i++)
                {
                    DestroyImmediate(colliders[i]);
                }

                BoxCollider[] cols = gameObject.GetComponents<BoxCollider>();
                for (int i = 0; i < cols.Length; i++)
                {
                    DestroyImmediate(cols[i]);
                }
            }
        }
    #endif*/

    BoxCollider GetCollider()
    {
        BoxCollider col = gameObject.AddComponent<BoxCollider>();
        col.hideFlags = HideFlags.HideInInspector;
        colliders.Add(col);
        return col;
    }
}
