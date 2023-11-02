using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
enum BreakType
{
    CHUNK,
    INDIVIDUAL
}

public class VoxelFracturer : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private BreakType breakType;
    [SerializeField] private float breakForce = 5f;
    [SerializeField] private float breakRadius = 3f;

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

                    Vector3 hitCentre = hit.point - (hit.normal * VoxelBuilder.HVoxelSize);

                    switch (vc.Renderer.BreakType)
                    {
                        case VoxelBreakType.PHYSICS:
                            Break(vc, hitCentre, ray.direction);
                            break;
                        case VoxelBreakType.SAND:
                            Dissolve(vc, hit.point);
                            break;
                    }
                    //Break(vc, hitCentre, ray.direction);
                }
            }
        }
    }

    void Dissolve(VoxelCollider vc, Vector3 hitCentre)
    {
        VoxelRenderer vr = vc.Renderer;
        List<VoxelPoint> n = FindAllNeighbors(vr, vr.ClosestPointTo(hitCentre),new List<VoxelPoint>( vr.VoxelData.VoxelPoints));
        Debug.Log(n.Count);
        StartCoroutine(DissolveIE(vr, n));
    }

    IEnumerator DissolveIE(VoxelRenderer vr, List<VoxelPoint> vps)
    {
        for (int i = vps.Count - 1; i >= 0; i--)
        {
            // remove from vr. create new
            List<VoxelPoint> vrVps = new List<VoxelPoint>(vr.VoxelData.VoxelPoints);
            vrVps.Remove(vps[i]);
            vr.BuildMesh(new VoxelData(vrVps.ToArray(), vr.VoxelData.Colors));

            VoxelBuilder.NewRenderer(new VoxelPoint[] { vps[i] }, vr.VoxelData.Colors, out _, vr.transform, vr.Material);
            yield return null;
        }
    }

    public List<VoxelPoint> FindAllNeighbors(VoxelRenderer vr, VoxelPoint startPoint, List<VoxelPoint> pointList)
    {
        List<VoxelPoint> result = new List<VoxelPoint>();
        List<VoxelPoint> visited = new List<VoxelPoint>();
        List<VoxelPoint> toVisit = new List<VoxelPoint>();

        toVisit.Add(startPoint);

        while (toVisit.Count > 0)
        {
            VoxelPoint currentPoint = toVisit[0];
            toVisit.RemoveAt(0);

            if (!visited.Contains(currentPoint))
            {
                visited.Add(currentPoint);
                result.Add(currentPoint);

                VoxelPoint[] neighbors = vr.GetNeighbours(currentPoint.Position, out int neighbourCount);

                for (int i = 0; i < neighbourCount; i++)
                {
                    if (pointList.Contains(neighbors[i]))
                    {
                        toVisit.Add(neighbors[i]);
                    }
                }
            }
        }

        return result;
    }

    void Break(VoxelCollider vc, Vector3 hitCentre, Vector3 dir)
    {
        VoxelPoint[] points = vc.Renderer.VoxelData.VoxelPoints;
        List<VoxelPoint> fractureChunk = new List<VoxelPoint>();
        List<VoxelPoint> cutChunk = new List<VoxelPoint>();

        for (int i = 0; i < points.Length; i++)
        {
            if (Vector3.Distance(vc.transform.InverseTransformPoint(hitCentre), points[i].LocalPosition) < breakRadius)
            {
                fractureChunk.Add(points[i]);
            }
            else
            {
                cutChunk.Add(points[i]);
            }
        }

        if (cutChunk.Count > 0)
        {
            VoxelData nVD = new VoxelData(cutChunk.ToArray(), vc.Renderer.VoxelData.Colors);
            vc.Renderer.BuildMesh(nVD);
        }

        if (vc.TryGetComponent(out Rigidbody rr))
        {
            rr.AddForce(dir * breakForce);
        }

        if (fractureChunk.Count > 0)
        {
            Rigidbody rig;
            if (fractureChunk.Count != points.Length)
            {
                VoxelBuilder.NewRenderer(fractureChunk.ToArray(), vc.Renderer.VoxelData.Colors, out rig, vc.transform, vc.Renderer.Material);
            }
            else
            {
                vc.TryGetComponent(out rig);
            }
            if (rig != null)
            {
                rig.AddForce(dir * breakForce);
            }
        }

        // Might blow up
        vc.Renderer.GroupAndFracture();
    }
}

