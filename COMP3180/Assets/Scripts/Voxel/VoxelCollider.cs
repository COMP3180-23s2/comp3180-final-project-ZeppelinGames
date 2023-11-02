using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(VoxelRenderer))]
public class VoxelCollider : MonoBehaviour
{
    private Dictionary<Vector3Int, BoxCollider> pointColliderMap = new Dictionary<Vector3Int, BoxCollider>();
    private Dictionary<BoxCollider, Vector3Int> colliderPointMap = new Dictionary<BoxCollider, Vector3Int>();

    private List<BoxCollider> colliderPool = new List<BoxCollider>();
    private VoxelRenderer voxRenderer;

    public VoxelRenderer Renderer
    {
        get
        {
            if (voxRenderer == null)
            {
                voxRenderer = GetComponent<VoxelRenderer>();
            }
            return voxRenderer;
        }
    }

    private void Start()
    {
        /*BoxCollider[] cols = gameObject.GetComponents<BoxCollider>();
        for (int i = 0; i < cols.Length; i++)
        {
            Destroy(cols[i]);
        }*/

        voxRenderer = GetComponent<VoxelRenderer>();
        voxRenderer.meshBuildComplete += MeshBuildEnd;
        BuildCollider();
    }

    void MeshBuildEnd()
    {
        BuildCollider();
    }

    private BoxCollider GetPooledCollider()
    {
        if (FirstDisabledCollider(out BoxCollider c))
        {
            return c;
        }
        else
        {
            BoxCollider bc = gameObject.AddComponent<BoxCollider>();
            bc.hideFlags = HideFlags.HideInInspector | HideFlags.HideAndDontSave;
            colliderPool.Add(bc);
            return bc;
        }
    }

    private bool FirstDisabledCollider(out BoxCollider c)
    {
        c = null;
        for (int i = 0; i < colliderPool.Count; i++)
        {
            if (!colliderPool[i].enabled)
            {
                c = colliderPool[i];
                c.enabled = true;
                return true;
            }
        }
        return false;
    }

    public void BuildCollider()
    {
        if(Renderer == null || Renderer.VoxelData == null)
        {
            return;
        }

        colliderPointMap.Clear();
        pointColliderMap.Clear();
        for (int i = 0; i < colliderPool.Count; i++)
        {
            colliderPool[i].enabled = false;
        }

        for (int i = 0; i < Renderer.VoxelData.VoxelPoints.Length; i++)
        {
            VoxelPoint vp = Renderer.VoxelData.VoxelPoints[i];
            if (pointColliderMap.ContainsKey(vp.Position))
            {
                BoxCollider bc = pointColliderMap[vp.Position];
                bc.enabled = true;
            }
            else
            {
                BoxCollider c = GetPooledCollider();
                AddToMap(c, vp.Position);
                c.center = vp.LocalPosition;
                c.size = new Vector3(0.9f, 0.9f, 0.9f) * VoxelBuilder.VoxelSize;
            }
        }
    }

    [ContextMenu("Reset Colliders")]
    private void ResetColliders()
    {
        BoxCollider[] bcs = gameObject.GetComponents<BoxCollider>();
        for (int i = 0; i < bcs.Length; i++)
        {
            DestroyImmediate(bcs[i]);
        }

        colliderPointMap.Clear();
        pointColliderMap.Clear();
        colliderPool.Clear();
    }

    private void AddToMap(BoxCollider bc, Vector3Int v)
    {
        if (!colliderPointMap.ContainsKey(bc))
        {
            colliderPointMap.Add(bc, v);
            pointColliderMap.Add(v, bc);
        }
    }

    private void RemoveFromMap(BoxCollider bc)
    {
        if (colliderPointMap.ContainsKey(bc))
        {
            pointColliderMap.Remove(colliderPointMap[bc]);
            colliderPointMap.Remove(bc);
        }
    }

    private void RemoveFromMap(Vector3Int v)
    {
        if (pointColliderMap.ContainsKey(v))
        {
            colliderPointMap.Remove(pointColliderMap[v]);
            pointColliderMap.Remove(v);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        for (int i = 0; i < colliderPool.Count; i++)
        {
            if (colliderPool[i].enabled)
            {
                Gizmos.DrawWireCube(colliderPool[i].center, colliderPool[i].size);
            }
        }
    }

#if UNITY_EDITOR
    public void ResetCollidersEditor()
    {
        for (int i = 0; i < colliderPool.Count; i++)
        {
            colliderPool[i].enabled = false;
        }
        colliderPointMap.Clear();
        pointColliderMap.Clear();
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
}
