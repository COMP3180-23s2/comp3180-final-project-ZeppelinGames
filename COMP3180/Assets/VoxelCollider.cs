using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(VoxelRenderer))]
public class VoxelCollider : MonoBehaviour
{
    private Dictionary<BoxCollider, VoxelPoint> colliderPoint = new Dictionary<BoxCollider, VoxelPoint>();
    private List<BoxCollider> colliders = new List<BoxCollider>();
    private VoxelRenderer renderer;

    public VoxelRenderer Renderer => renderer;

    private void Start()
    {
        UpdateCollider();
    }

    private void OnValidate()
    {
        UpdateCollider();
    }

    void UpdateCollider()
    {
        if (renderer == null)
        {
            renderer = GetComponent<VoxelRenderer>();
            return;
        }
        if(renderer.VoxelData == null || renderer.VoxelData.VoxelPoints == null)
        {
            return;
        }

        VoxelPoint[] points = renderer.VoxelData.VoxelPoints;
        for (int i = 0; i < colliders.Count; i++)
        {
            colliders[i].enabled = false;
        }

        for (int i = 0; i < points.Length; i++)
        {
            BoxCollider c = PoolCollider();
            c.center = points[i].WorldPosition;
            c.size = Vector3.one * VoxelBuilder.VoxelSize;
        }
    }

    BoxCollider PoolCollider()
    {
        if (TryGetPoolCollider(out BoxCollider c))
        {
            return c;
        }
        else
        {
            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.hideFlags = HideFlags.HideInInspector;
            colliders.Add(col);
            return col;
        }
    }

    bool TryGetPoolCollider(out BoxCollider c)
    {
        for (int i = 0; i < colliders.Count; i++)
        {
            if (!colliders[i].enabled)
            {
                c = colliders[i];
                return true;
            }
        }
        c = null;
        return false;
    }
}
