using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelSelector : MonoBehaviour
{
    [SerializeField] private Camera cam;

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -10f;
        Ray ray = cam.ScreenPointToRay(mousePos);
        
        // see if we hit voxel collider
    }
}
