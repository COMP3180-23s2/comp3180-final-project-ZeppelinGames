using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCutter : MonoBehaviour
{
    [SerializeField] private GameObject cut;
    [SerializeField] private int maxCuts;

    private LayerMask cutMeshLayer;

    // Start is called before the first frame update
    void Start()
    {
        cutMeshLayer = LayerMask.NameToLayer("CutMesh");
        RecurseCut(cut, maxCuts);
    }

    void RecurseCut(GameObject cut, int cutCount)
    {
        if(cutCount <= 0)
        {
            return;
        }

        MeshRenderer mr = cut.GetComponent<MeshRenderer>();
        Vector3 boundsCenter = mr.bounds.center;

        if (Cutter.Cut(cut, boundsCenter, Random.insideUnitSphere, out GameObject right))
        {
            cutCount -= 1;
            right.layer = cutMeshLayer;
            RecurseCut(cut, cutCount);
            RecurseCut(right, cutCount);
        }
    }
}
