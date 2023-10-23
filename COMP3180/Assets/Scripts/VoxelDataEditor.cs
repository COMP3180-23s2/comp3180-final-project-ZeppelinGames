using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class VoxelDataEditor : MonoBehaviour
{
    private VoxelRenderer voxel;

    private Vector3Int addedVoxels;
    private bool hadCollider;

    public void SetVoxel(VoxelRenderer renderer)
    {
        voxel = renderer;

        if (voxel.TryGetComponent(out VoxelCollider vc))
        {
            hadCollider = true;
            Debug.Log("Has collider");
        }
        else
        {
            hadCollider = false;
            VoxelCollider vcAdded = voxel.gameObject.AddComponent<VoxelCollider>();
            vcAdded.RefreshCollider();
        }
    }

    private void OnDisable()
    {
        if (!hadCollider)
        {
            VoxelCollider vc = voxel.gameObject.GetComponent<VoxelCollider>();
            vc.ResetCollidersEditor();
            DestroyImmediate(vc);
        }
    }

    // Update is called once per frame
    void OnDrawGizmos()
    {
        if (voxel == null || voxel.VoxelData == null)
        {
            return;
        }

        for (int i = 0; i < voxel.VoxelData.VoxelPoints.Length; i++)
        {
            Gizmos.color = voxel.VoxelData.Colors[voxel.VoxelData.VoxelPoints[i].ColorIndex];
            Gizmos.DrawCube(voxel.VoxelData.VoxelPoints[i].LocalPosition, Vector3.one * VoxelBuilder.VoxelSize);
        }
    }
}