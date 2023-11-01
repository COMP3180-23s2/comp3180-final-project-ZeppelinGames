﻿using System.Collections;
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

                    Break(vc, hitCentre, ray.direction);
                    //Dissolve(vc, hit.point);
                }
            }
        }
    }

    void Dissolve(VoxelCollider vc, Vector3 hitCentre)
    {
        StartCoroutine(DissolveIE(
            vc.Renderer, 
            vc.Renderer.ClosestPointTo(hitCentre), 
            new HashSet<VoxelPoint>()));
    }

    IEnumerator DissolveIE(VoxelRenderer vr, VoxelPoint vp, HashSet<VoxelPoint> dissolving)
    {
        Debug.Log(vp.Position);
        VoxelPoint[] vps = vr.GetNeighbours(vp.Position, out int nCount);
        List<VoxelPoint> toDissolve = new List<VoxelPoint>();
        for (int i = 0; i < nCount; i++)
        {
            if (!dissolving.Contains(vps[i]))
            {
                toDissolve.Add(vps[i]);
                dissolving.Add(vps[i]);
            }
        }

        Debug.Log(nCount);

        for (int i = 0; i < toDissolve.Count; i++)
        {
            VoxelBuilder.NewRenderer(new VoxelPoint[] { toDissolve[i] }, vr.VoxelData.Colors, out _, vr.transform);
            DissolveIE(vr, toDissolve[i], dissolving);
            yield return null;
        }
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
                VoxelBuilder.NewRenderer(fractureChunk.ToArray(), vc.Renderer.VoxelData.Colors, out rig, vc.transform);
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

