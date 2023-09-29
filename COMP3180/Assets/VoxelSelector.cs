using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelSelector : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private int fractureDepth = 3;

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -10f;
        Ray ray = cam.ScreenPointToRay(mousePos);

        // see if we hit voxel collider
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent(out VoxelCollider vc))
            {

            }
        }
    }
}
