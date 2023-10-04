using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
enum BreakType
{
    CHUNK,
    INDIVIDUAL
}

public class VoxelSelector : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private BreakType breakType;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -10f;
            Ray ray = cam.ScreenPointToRay(mousePos);

            // see if we hit voxel collider
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out VoxelCollider vc))
                {
                    if (vc.Renderer == null || vc.Renderer.VoxelData == null)
                    {
                        Debug.Log("Null data");
                        return;
                    }

                    VoxelPoint[] points = vc.Renderer.VoxelData.VoxelPoints;
                    List<VoxelPoint> fractureChunk = new List<VoxelPoint>();
                    List<VoxelPoint> cutChunk = new List<VoxelPoint>();
                    for (int i = 0; i < points.Length; i++)
                    {
                        if (Vector3.Distance(hit.point, points[i].WorldPosition) < 1f)
                        {
                            fractureChunk.Add(points[i]);
                        }
                        else
                        {
                            cutChunk.Add(points[i]);
                        }
                    }

                    vc.Renderer.BuildMesh(new VoxelData(cutChunk.ToArray(), vc.Renderer.VoxelData.Colors));
                    
                    newRenderer(fractureChunk.ToArray(), vc.Renderer.VoxelData.Colors);
                }
            }
        }
    }

    VoxelRenderer newRenderer(VoxelPoint[] points, Color[] cols) 
    {
        VoxelRenderer rend = new GameObject().AddComponent<VoxelRenderer>();
        rend.BuildMesh(new VoxelData(points, cols));

        rend.gameObject.AddComponent<VoxelCollider>();
        rend.gameObject.AddComponent<Rigidbody>();

        return rend;
    }
}
